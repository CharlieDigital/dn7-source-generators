using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Text.Json;
using System;

namespace generator;

/// <summary>
/// Classes marked with JSON+LD schema.
/// </summary>
public class JsonLdSyntaxReceiver : ISyntaxContextReceiver {
  public List<(string Namespace, string ClassName, string Json)> Models = new();
  public List<string> Log = new();

  public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
    if (context.Node is not ClassDeclarationSyntax classDec) {
      return;
    }

    var classNode = context.SemanticModel.GetDeclaredSymbol(context.Node);

    var jsonLdAttribute = classNode
      .GetAttributes()
      .FirstOrDefault(a => a.AttributeClass.Name == "JsonLdSourceAttribute");

    Log.Add($"{classNode.Name}: {jsonLdAttribute}");

    if (jsonLdAttribute is not null) {
      Log.Add($"  JSON: {jsonLdAttribute.ConstructorArguments[0].Value}");

      Models.Add((
        (classDec.Parent as FileScopedNamespaceDeclarationSyntax).Name.ToFullString(),
        classDec.Identifier.ToString(),
        jsonLdAttribute.ConstructorArguments[0].Value.ToString())
      );
    }
  }
}

/// <summary>
/// This is our generator that actually outputs the source code.
/// </summary>
[Generator]
public class JsonLdGenerator : ISourceGenerator {
  /// <summary>
  /// We hook up our receiver here.
  /// </summary>
  public void Initialize(GeneratorInitializationContext context) {
    context.RegisterForSyntaxNotifications(() => new JsonLdSyntaxReceiver());
  }

  /// <summary>
  /// And consume the receiver here.
  /// </summary>
  public void Execute(GeneratorExecutionContext context) {
    var models = (context.SyntaxContextReceiver as JsonLdSyntaxReceiver).Models;

    foreach (var modelClass in models) {
      var classBuffer = new StringBuilder();
      var src = new StringBuilder();
      src.AppendLine($@"
using System.Text.Json;
using System.Text.Json.Serialization;

namespace {modelClass.Namespace};

public partial class {modelClass.ClassName} {{
  public void Test() {{
    Console.WriteLine(""{modelClass.Namespace}"");
  }}");

      // Read the JSON and create properties.
      var json = JsonDocument.Parse(modelClass.Json);

      src.Append(ResolveObject(classBuffer, "", json.RootElement));

      src.AppendLine("}");

      src.Append(classBuffer.ToString());

      context.AddSource($"{modelClass.ClassName}.g.cs", src.ToString());
    }

    // Generate log file for testing.
    var logs = new StringBuilder();

    logs.AppendLine("/*");

    (context.SyntaxContextReceiver as JsonLdSyntaxReceiver).Log
      .Aggregate(logs, (buffer, log) => {
        buffer.AppendLine(log);

        return buffer;
      });

    logs.AppendLine("*/");

    context.AddSource("Logs", logs.ToString());
  }

  /// <summary>
  /// Resolves an array.  Need to determine if it's an array of primitives
  /// or an array of objects.
  /// </summary>
  /// <param name="propName">The property name</param>
  /// <param name="element">The JSON node element to process</param>
  /// <returns>A string which either represents a primitive array or an object array including a class definition for the object.</returns>
  private string ResolveArray(string propName, JsonElement element) {
    var src = new StringBuilder();

    src.Append("public ");

    foreach (var item in element.EnumerateArray()) {
      if (item.ValueKind == JsonValueKind.String) {
        src.Append($"string[] {propName} {{ get; set; }} = Array.Empty<string>();");
        break;
      } else if (item.ValueKind == JsonValueKind.Number) {
        src.Append($"decimal[] {propName} {{ get; set; }} = Array.Empty<decimal>();");
        break;
      }
    }

    return src.ToString();
  }

  /// <summary>
  /// Resolves an object from the JSON+LD
  /// </summary>
  /// <param name="propName">The property name</param>
  /// <param name="element">The JSON node element to process</param>
  /// <returns>The source and a class if the object defines a class type.</returns>
  private string ResolveObject(
      StringBuilder classBuffer,
      string propName,
      JsonElement element
    ) {
      var makeUpper = (string s) => {
        s = s.Replace("@", "");
        return $"{s[..1].ToUpper()}{s[1..]}";
      };

      var src = new StringBuilder();

      var accumulateClass = "";

      foreach(var prop in element.EnumerateObject()) {
        var child = element.GetProperty(prop.Name);

        var property = child.ValueKind switch {
          JsonValueKind.String => $"public string {makeUpper(prop.Name)} {{ get; set;}} = \"\";",
          JsonValueKind.Number => $"public decimal {makeUpper(prop.Name)} {{ get; set; }}",
          JsonValueKind.False => $"public bool {makeUpper(prop.Name)} {{ get; set; }}",
          JsonValueKind.True => $"public bool {makeUpper(prop.Name)} {{ get; set; }}",
          // JsonValueKind.Null => $"public object? {makeUpper(prop.Name)} {{ get; set; }}",
          // JsonValueKind.Undefined => $"public object? {makeUpper(prop.Name)} {{ get; set; }}",
          JsonValueKind.Array => ResolveArray(makeUpper(prop.Name), child),
          JsonValueKind.Object => ResolveObject(classBuffer, makeUpper(prop.Name), child),
          _ => $"// Skip: {prop.Name}, {child.ValueKind}"
        };

        if (!property.StartsWith("//")) {
          property = $"  [JsonPropertyName(\"{prop.Name}\")] {property}";
        }

        // Check if we need to register a class based on the discovery of a type.
        if (!string.IsNullOrEmpty(propName) && prop.Name == "@type") {
          classBuffer.AppendLine();
          classBuffer.AppendLine("/// <summary>Generated class</summary>");
          classBuffer.AppendLine(@$"public class {prop.Value} {{");

          accumulateClass = prop.Value.ToString();
        }

        // We'll need to append the property to the class buffer instead since we're
        // building up a class.  This REQUIRES that the @type is the first property.
        // Or we can extract the enumerator to a list first.
        if (string.IsNullOrEmpty(accumulateClass)) {
          src.AppendLine(property);
        } else {
          classBuffer.AppendLine(property);
        }
      }

      if (!string.IsNullOrEmpty(accumulateClass)) {
        classBuffer.AppendLine("}");

        // Need this to fix: "warning CS8669: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context. Auto-generated code requires an explicit '#nullable' directive in source."
        src.AppendLine();
        src.AppendLine("  #nullable enable");
        src.AppendLine($"  public {accumulateClass}? {propName} {{ get; set; }}");
        src.AppendLine("  #nullable disable");
      }

      return src.ToString();
  }
}

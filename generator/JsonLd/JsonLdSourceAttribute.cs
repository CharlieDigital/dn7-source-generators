using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace generator;

/// <summary>
/// Generates the attribute that we used to decorate classes for the JSON model.
/// </summary>
[Generator]
public sealed class JsonLdSourceAttributeGenerator : IIncrementalGenerator {
  public void Initialize(IncrementalGeneratorInitializationContext context) =>
    context.RegisterPostInitializationOutput(
      context =>
        context.AddSource(
          $"{JsonLdSourceAttributeSource.Name}.g.cs",
          SourceText.From(JsonLdSourceAttributeSource.SourceCode, Encoding.UTF8)
        )
    );
}

/// <summary>
/// Source generator for the JSON attribute
/// </summary>
internal static class JsonLdSourceAttributeSource {
  public const string Namespace = "generator";
  public const string Name = "JsonLdSourceAttribute";
  public const string FullyQualifiedName = $"{Namespace}.{Name}";

  public const string SourceCode = """
using System;
using System.Reflection;

namespace generator;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class JsonLdSourceAttribute : Attribute {
  public JsonLdSourceAttribute(string json) {

  }
}
""";
}
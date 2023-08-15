using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace generator {
  /// <summary>
  /// This will receive each "syntax context" to determine if we want to act on it.
  /// Here, we'll capture a list of models that implement Entity.
  /// </summary>
  public class RepoSyntaxReceiver : ISyntaxContextReceiver {
    public List<string> Models = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
      if (context.Node is not ClassDeclarationSyntax classDec
        || classDec.BaseList == null) {
        return;
      }

      if (classDec.BaseList.Types.Any(t => t.ToString() == "Entity")) {
        Models.Add(classDec.Identifier.ToString());
      }
    }
  }

  /// <summary>
  /// This is our generator that actually outputs the source code.
  /// </summary>
  [Generator]
  public class RepoGenerator : ISourceGenerator {
    /// <summary>
    /// We hook up our receiver here.
    /// </summary>
    public void Initialize(GeneratorInitializationContext context) {
      context.RegisterForSyntaxNotifications(() => new RepoSyntaxReceiver());
    }

    /// <summary>
    /// And consume the receiver here.
    /// </summary>
    public void Execute(GeneratorExecutionContext context) {
      var models = (context.SyntaxContextReceiver as RepoSyntaxReceiver).Models;

      foreach (var modelClass in models) {
        var src = $@"
using System;

namespace runtime;

public partial class {modelClass}Repository : RepositoryBase<{modelClass}> {{

}}";
        context.AddSource($"{modelClass}Repository.g.cs", src);
      }
    }

  }
}

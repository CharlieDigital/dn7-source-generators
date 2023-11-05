using generator;

namespace runtime.models;
[JsonLdSource(SampleJsonLd.Json)]
public partial class SampleJsonLd {

  /// <summary>
  /// This const string provides the template JSON since we can't
  /// load it from the file system.
  /// </summary>
  public const string Json = @"{
    ""message"": ""Hello, World""
  }";

}


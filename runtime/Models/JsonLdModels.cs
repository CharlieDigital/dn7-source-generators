using generator;

namespace runtime.models;
[JsonLdSource(Json)]
public partial class SampleJsonLd {

  /// <summary>
  /// This const string provides the template JSON since we can't
  /// load it from the file system.
  /// </summary>
  public const string Json = @"{
      ""@context"": ""https://schema.org"",
      ""@type"": ""Person"",
      ""address"": {
        ""@type"": ""PostalAddress"",
        ""addressLocality"": ""Colorado Springs"",
        ""addressRegion"": ""CO"",
        ""postalCode"": ""80840"",
        ""streetAddress"": ""100 Main Street""
      },
      ""colleague"": [
        ""http://www.example.com/JohnColleague.html"",
        ""http://www.example.com/JameColleague.html""
      ],
      ""email"": ""info@example.com"",
      ""image"": ""janedoe.jpg"",
      ""jobTitle"": ""Research Assistant"",
      ""name"": ""Jane Doe"",
      ""alumniOf"": ""Dartmouth"",
      ""birthPlace"": ""Philadelphia, PA"",
      ""birthDate"": ""1979-10-12"",
      ""height"": ""72 inches"",
      ""gender"": ""female"",
      ""memberOf"": ""Democratic Party"",
      ""nationality"": ""Albanian"",
      ""telephone"": ""(123) 456-6789"",
      ""url"": ""http://www.example.com"",
	    ""sameAs"" : [
        ""https://www.facebook.com/"",
        ""https://www.linkedin.com/"",
        ""http://twitter.com/"",
        ""http://instagram.com/"",
        ""https://plus.google.com/""
      ]
    }";

}


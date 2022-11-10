using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Utils;

namespace UtilsTest;

[TestClass]
[ExcludeFromCodeCoverage]
public class SemanticVersionRangeSetTest {
	readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions {
		Converters = {
			new IParsableJsonCoverterFactory()
		}
	};

	[TestMethod]
	public void Main() {
		SemanticVersionRangeSet semanticVersionRangeSet1 = new SemanticVersionRangeSet(">=1.0.0||<0.1.0");
		SemanticVersionRangeSet semanticVersionRangeSet2 = new SemanticVersionRangeSet(new List<SemanticVersionRange> { new SemanticVersionRange(">=1.0.0"), new SemanticVersionRange("<0.1.0") });
		SemanticVersionRangeSet semanticVersionRangeSet3 = new SemanticVersionRangeSet("<0.1.0||>=1.0.0");
		Assert.IsTrue(semanticVersionRangeSet1.Includes(new SemanticVersion("0.0.0")));
		Assert.IsTrue(semanticVersionRangeSet2.Includes(new SemanticVersion("0.0.0")));
		Assert.IsFalse(semanticVersionRangeSet1.Includes(new SemanticVersion("0.1.0")));
		Assert.IsFalse(semanticVersionRangeSet2.Includes(new SemanticVersion("0.1.0")));
		Assert.IsFalse(semanticVersionRangeSet1.Includes(new SemanticVersion("0.2.0")));
		Assert.IsFalse(semanticVersionRangeSet2.Includes(new SemanticVersion("0.2.0")));
		Assert.IsTrue(semanticVersionRangeSet1.Includes(new SemanticVersion("1.0.0")));
		Assert.IsTrue(semanticVersionRangeSet2.Includes(new SemanticVersion("1.0.0")));

		Assert.IsTrue(semanticVersionRangeSet1.Equals(semanticVersionRangeSet2));
		Assert.IsTrue(semanticVersionRangeSet1 == semanticVersionRangeSet2);
		Assert.IsFalse(semanticVersionRangeSet1 != semanticVersionRangeSet2);
		Assert.IsTrue(semanticVersionRangeSet1.Equals(semanticVersionRangeSet3));
		Assert.IsTrue(semanticVersionRangeSet1 == semanticVersionRangeSet3);
		Assert.IsFalse(semanticVersionRangeSet1 != semanticVersionRangeSet3);

		Assert.IsTrue(semanticVersionRangeSet1.ToString().Equals(semanticVersionRangeSet2.ToString()));
		Assert.IsFalse(semanticVersionRangeSet1.ToString().Equals(semanticVersionRangeSet3.ToString()));
		Assert.IsTrue(semanticVersionRangeSet1.GetHashCode().Equals(semanticVersionRangeSet2.GetHashCode()));
		Assert.IsTrue(semanticVersionRangeSet1.GetHashCode().Equals(semanticVersionRangeSet3.GetHashCode()));

		Assert.IsFalse(semanticVersionRangeSet1.Equals(new SemanticVersionRangeSet(">=1.0.0||<0.1.0||=0.2.65")));
		Assert.IsFalse(semanticVersionRangeSet1.Equals((object)new SemanticVersionRangeSet(">=1.0.0||<0.1.0||=0.2.65")));
		Assert.IsFalse(semanticVersionRangeSet1.Equals(new SemanticVersionRangeSet("*")));
		Assert.IsFalse(semanticVersionRangeSet1.Equals((object)new SemanticVersionRangeSet("*")));
		Assert.IsFalse(semanticVersionRangeSet1.ToString().Equals(new SemanticVersionRangeSet("*").ToString()));
		Assert.IsFalse(semanticVersionRangeSet1.ToString().Equals((object)new SemanticVersionRangeSet("*").ToString()));
		Assert.IsFalse(semanticVersionRangeSet1.GetHashCode().Equals(new SemanticVersionRangeSet("*").GetHashCode()));
		Assert.IsFalse(semanticVersionRangeSet1.GetHashCode().Equals((object)new SemanticVersionRangeSet("*").GetHashCode()));
		Assert.IsFalse(semanticVersionRangeSet1.Equals(0));
		Assert.IsFalse(semanticVersionRangeSet1.Equals(""));
		Assert.IsFalse(semanticVersionRangeSet1.Equals(null));
		Assert.IsFalse(semanticVersionRangeSet1.Equals((object)null));
		Assert.IsFalse(semanticVersionRangeSet1 == null);
		Assert.IsFalse(null == semanticVersionRangeSet1);
		#pragma warning disable IDE0004
		Assert.IsTrue((SemanticVersionRangeSet)null == (SemanticVersionRangeSet)null);
		#pragma warning restore IDE0004
		Assert.IsTrue(semanticVersionRangeSet1 != null);
	}

	[TestMethod]
	public void JsonConverter() {
		string str = "{\"semanticVersionRangeSet\":\"\\u003E=1.2.3-4\\u002B5||*\"}";
		JsonConverterTest test = JsonSerializer.Deserialize<JsonConverterTest>(str, jsonSerializerOptions);
		Assert.IsTrue(test.semanticVersionRangeSet == new SemanticVersionRangeSet(">=1.2.3-4+5||*"));
		Assert.IsTrue(JsonSerializer.Serialize(test, jsonSerializerOptions) == str, $"{JsonSerializer.Serialize(test, jsonSerializerOptions)}");
	}

	public class JsonConverterTest {
		public SemanticVersionRangeSet semanticVersionRangeSet { get; set; }
	}
}
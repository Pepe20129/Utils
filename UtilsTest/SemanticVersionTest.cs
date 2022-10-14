using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.Json;
using Utils;

namespace UtilsTest;

[TestClass]
[ExcludeFromCodeCoverage]
public class SemanticVersionTest {
	private readonly string[,] versions = new string[,] {
		{ "0.0.0", "0.0.1" },
		{ "0.0.0", "0.1.0" },
		{ "0.0.1", "0.1.0" },
		{ "0.0.0-agfbndb", "0.0.0" },
		{ "0.0.0", "0.0.1-0" },
		{ "0.0.0", "0.0.1+hdhwgf" },
		{ "0.0.0+hdhwgf", "0.0.1" },
		{ "0.0.0-0", "0.0.0-0.0" },
		{ "0.0.0-0", "0.0.0-a" },
		{ "0.0.0-0", "0.0.0-1"},
		{ "0.0.0-a", "0.0.0-b" },
		{ "0.0.0-a", "0.0.0-aa"}
	};
	readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions {
		Converters = {
			new StringConstructorJsonCoverterFactory()
		}
	};

	#region Operators

	[TestMethod]
	public void SmallerThan() {
		for (int i=0;i<versions.GetLength(0);i+=1) {
			SemanticVersion smaller = new SemanticVersion(versions[i, 0]);
			SemanticVersion bigger = new SemanticVersion(versions[i, 1]);
			Assert.IsTrue(smaller < bigger);
			Assert.IsFalse(bigger < smaller);
			#pragma warning disable CS1718 // Comparison made to same variable
			Assert.IsFalse(smaller < smaller);
			Assert.IsFalse(bigger < bigger);
			#pragma warning restore CS1718 // Comparison made to same variable
			Assert.IsTrue(null < smaller);
			Assert.IsTrue(null < bigger);

			Assert.IsFalse(smaller < null);
			Assert.IsFalse(bigger < null);
		}
	}

	[TestMethod]
	public void SmallerThanOrEqual() {
		string[,] versions2 = new string[,] {
			{ "0.0.0+hetdf", "0.0.0" },
			{ "0.0.0-egrdf", "0.0.0-egrdf+egdvs" }
		};
		for (int i=0;i<versions.GetLength(0)+versions2.GetLength(0); i+=1) {
			SemanticVersion smaller;
			SemanticVersion bigger;
			if (i<versions.GetLength(0)) {
				smaller = new SemanticVersion(versions[i, 0]);
				bigger = new SemanticVersion(versions[i, 1]);
				Assert.IsFalse(bigger <= smaller);
			} else {
				smaller = new SemanticVersion(versions2[i-versions.GetLength(0), 0]);
				bigger = new SemanticVersion(versions2[i-versions.GetLength(0), 1]);
				Assert.IsTrue(bigger <= smaller);
			}
			
			Assert.IsTrue(smaller <= bigger);
			
			#pragma warning disable CS1718 // Comparison made to same variable
			Assert.IsTrue(smaller <= smaller);
			Assert.IsTrue(bigger <= bigger);
			#pragma warning restore CS1718 // Comparison made to same variable
			Assert.IsTrue(null <= smaller);
			Assert.IsTrue(null <= bigger);

			Assert.IsFalse(smaller <= null);
			Assert.IsFalse(bigger <= null);
		}
	}

	[TestMethod]
	public void GreaterThan() {
		for (int i=0;i<versions.GetLength(0);i+=1) {
			SemanticVersion smaller = new SemanticVersion(versions[i, 0]);
			SemanticVersion bigger = new SemanticVersion(versions[i, 1]);
			Assert.IsTrue(bigger > smaller);
			Assert.IsFalse(smaller > bigger);
			#pragma warning disable CS1718 // Comparison made to same variable
			Assert.IsFalse(smaller > smaller);
			Assert.IsFalse(bigger > bigger);
			#pragma warning restore CS1718 // Comparison made to same variable
			Assert.IsTrue(smaller > null);
			Assert.IsTrue(bigger > null);

			Assert.IsFalse(null > smaller);
			Assert.IsFalse(null > bigger);
		}
	}

	[TestMethod]
	public void GreaterThanOrEqual() {
		string[,] versions2 = new string[,] {
			{ "0.0.0+hetdf", "0.0.0" },
			{ "0.0.0-egrdf", "0.0.0-egrdf+egdvs" }
		};
		for (int i=0;i<versions.GetLength(0)+versions2.GetLength(0); i+=1) {
			SemanticVersion smaller;
			SemanticVersion bigger;
			if (i < versions.GetLength(0)) {
				smaller = new SemanticVersion(versions[i, 0]);
				bigger = new SemanticVersion(versions[i, 1]);
				Assert.IsFalse(smaller >= bigger);
			} else {
				smaller = new SemanticVersion(versions2[i-versions.GetLength(0), 0]);
				bigger = new SemanticVersion(versions2[i-versions.GetLength(0), 1]);
				Assert.IsTrue(smaller >= bigger);
			}

			Assert.IsTrue(bigger >= smaller);
			
			#pragma warning disable CS1718 // Comparison made to same variable
			Assert.IsTrue(smaller >= smaller);
			Assert.IsTrue(bigger >= bigger);
			#pragma warning restore CS1718 // Comparison made to same variable
			Assert.IsTrue(smaller >= null);
			Assert.IsTrue(bigger >= null);

			Assert.IsFalse(null >= smaller);
			Assert.IsFalse(null >= bigger);
		}
	}

	[TestMethod]
	public void Equality() {
		for (int i=0;i<versions.GetLength(0);i+=1) {
			SemanticVersion sv1 = new SemanticVersion(versions[i, 0]);
			SemanticVersion sv2 = new SemanticVersion(versions[i, 1]);
			#pragma warning disable CS1718 // Comparison made to same variable
			Assert.IsTrue(sv1 == sv1);
			Assert.IsTrue(sv2 == sv2);
			#pragma warning restore CS1718 // Comparison made to same variable
			Assert.IsFalse(sv1 == sv2);
			Assert.IsFalse(sv2 == sv1);

			Assert.IsFalse(sv1 == null);
			Assert.IsFalse(sv2 == null);

			Assert.IsFalse(null == sv1);
			Assert.IsFalse(null == sv2);

			Assert.AreEqual(sv1, sv1);
			Assert.AreEqual(sv2, sv2);
			Assert.AreNotEqual(sv1, sv2);
			Assert.AreNotEqual(sv2, sv1);

			Assert.IsTrue(sv1.Equals(sv1));
			Assert.IsTrue(sv2.Equals(sv2));
			Assert.IsFalse(sv1.Equals(sv2));
			Assert.IsFalse(sv2.Equals(sv1));

			Assert.IsFalse(sv1.Equals(""));
			Assert.IsFalse(sv2.Equals(""));
			Assert.IsFalse(sv1.Equals(0));
			Assert.IsFalse(sv2.Equals(0));
			Assert.IsFalse(sv1.Equals(false));
			Assert.IsFalse(sv2.Equals(false));
		}
	}

	[TestMethod]
	public void Inequality() {
		for (int i=0;i<versions.GetLength(0);i+=1) {
			SemanticVersion sv1 = new SemanticVersion(versions[i, 0]);
			SemanticVersion sv2 = new SemanticVersion(versions[i, 1]);
			#pragma warning disable CS1718 // Comparison made to same variable
			Assert.IsFalse(sv1 != sv1);
			Assert.IsFalse(sv2 != sv2);
			#pragma warning restore CS1718 // Comparison made to same variable
			Assert.IsTrue(sv1 != sv2);
			Assert.IsTrue(sv2 != sv1);

			Assert.IsTrue(sv1 != null);
			Assert.IsTrue(sv2 != null);

			Assert.IsTrue(null != sv1);
			Assert.IsTrue(null != sv2);
		}
	}

	#endregion

	[TestMethod]
	public void StringConversion() {
		for (int i = 0; i < versions.GetLength(0); i += 1) {
			SemanticVersion sv1 = new SemanticVersion(versions[i, 0]);
			SemanticVersion sv2 = new SemanticVersion(versions[i, 1]);
			Assert.IsTrue(versions[i,0] == sv1.ToString());
			Assert.IsTrue(versions[i,1] == sv2.ToString());
			Assert.IsTrue(versions[i, 0] == sv1);
			Assert.IsTrue(versions[i, 1] == sv2);
		}
	}

	[TestMethod]
	public void Initialization() {
		object[,] valid = new object[,] {
			{ "0.0.4", 0, 0, 4, "", "" },
			{ "1.2.3", 1, 2, 3, "", "" },
			{ "10.20.30", 10, 20, 30, "", "" },
			{ "1.1.2-prerelease+meta", 1, 1, 2, "prerelease", "meta" },
			{ "1.1.2+meta", 1, 1, 2, "", "meta" },
			{ "1.1.2+meta-valid", 1, 1, 2, "", "meta-valid" },
			{ "1.0.0-alpha", 1, 0, 0, "alpha", "" },
			{ "1.0.0-beta", 1, 0, 0, "beta", "" },
			{ "1.0.0-alpha.beta", 1, 0, 0, "alpha.beta", "" },
			{ "1.0.0-alpha.beta.1", 1, 0, 0, "alpha.beta.1", "" },
			{ "1.0.0-alpha.1", 1, 0, 0, "alpha.1", "" },
			{ "1.0.0-alpha0.valid", 1, 0, 0, "alpha0.valid", "" },
			{ "1.0.0-alpha.0valid", 1, 0, 0, "alpha.0valid", "" },
			{ "1.0.0-alpha-a.b-c-somethinglong+build.1-aef.1-its-okay", 1, 0, 0, "alpha-a.b-c-somethinglong", "build.1-aef.1-its-okay" },
			{ "1.0.0-rc.1+build.1", 1, 0, 0, "rc.1", "build.1" },
			{ "2.0.0-rc.1+build.123", 2, 0, 0, "rc.1", "build.123" },
			{ "1.2.3-beta", 1, 2, 3, "beta", "" },
			{ "10.2.3-DEV-SNAPSHOT", 10, 2, 3, "DEV-SNAPSHOT", "" },
			{ "1.2.3-SNAPSHOT-123", 1, 2, 3, "SNAPSHOT-123", "" },
			{ "1.0.0", 1, 0, 0, "", "" },
			{ "2.0.0", 2, 0, 0, "", "" },
			{ "1.1.7", 1, 1, 7, "", "" },
			{ "2.0.0+build.1848", 2, 0, 0, "", "build.1848" },
			{ "2.0.1-alpha.1227", 2, 0, 1, "alpha.1227", "" },
			{ "1.0.0-alpha+beta", 1, 0, 0, "alpha", "beta" },
			{ "1.2.3----RC-SNAPSHOT.12.9.1--.12+788", 1, 2, 3, "---RC-SNAPSHOT.12.9.1--.12", "788" },
			{ "1.2.3----R-S.12.9.1--.12+meta", 1, 2, 3, "---R-S.12.9.1--.12", "meta" },
			{ "1.2.3----RC-SNAPSHOT.12.9.1--.12", 1, 2, 3, "---RC-SNAPSHOT.12.9.1--.12", "" },
			{ "1.0.0+0.build.1-rc.10000aaa-kk-0.1", 1, 0, 0, "", "0.build.1-rc.10000aaa-kk-0.1" },
			{ "99999999999999999999999.999999999999999999.99999999999999999", BigInteger.Parse("99999999999999999999999"), BigInteger.Parse("999999999999999999"), BigInteger.Parse("99999999999999999"), "", "" },
			{ "1.0.0-0A.is.legal", 1, 0, 0, "0A.is.legal", "" },
			{ "1.0.0-rc.2", 1, 0, 0, "rc.2", "" },
			{ "1.0.0-z", 1, 0, 0, "z", "" }
		};
		for (int i = 0; i < valid.GetLength(0); i += 1) {
			Assert.IsTrue(new SemanticVersion(valid[i, 0].ToString()) == new SemanticVersion(BigInteger.Parse(valid[i, 1].ToString()), BigInteger.Parse(valid[i, 2].ToString()), BigInteger.Parse(valid[i, 3].ToString()), valid[i, 4].ToString(), valid[i, 5].ToString()));
			Assert.IsTrue(new SemanticVersion(valid[i, 0].ToString()).GetHashCode() == new SemanticVersion(BigInteger.Parse(valid[i, 1].ToString()), BigInteger.Parse(valid[i, 2].ToString()), BigInteger.Parse(valid[i, 3].ToString()), valid[i, 4].ToString(), valid[i, 5].ToString()).GetHashCode());
			Assert.IsTrue(new SemanticVersion(valid[i, 0].ToString()).major == new SemanticVersion(valid[i, 0].ToString()).ToImmutable().major);
			Assert.IsTrue(new SemanticVersion(valid[i, 0].ToString()).minor == new SemanticVersion(valid[i, 0].ToString()).ToImmutable().minor);
			Assert.IsTrue(new SemanticVersion(valid[i, 0].ToString()).patch == new SemanticVersion(valid[i, 0].ToString()).ToImmutable().patch);
			Assert.IsTrue(new SemanticVersion(valid[i, 0].ToString()).preRelease == new SemanticVersion(valid[i, 0].ToString()).ToImmutable().preRelease);
			Assert.IsTrue(new SemanticVersion(valid[i, 0].ToString()).buildMetadata == new SemanticVersion(valid[i, 0].ToString()).ToImmutable().buildMetadata);
		}

		string[] invalid = new string[] {
			"1",
			"1.2",
			"1.2.3-0123",
			"1.2.3-0123.0123",
			"1.1.2+.123",
			"+invalid",
			"-invalid",
			"-invalid+invalid",
			"-invalid.01",
			"alpha",
			"alpha.beta",
			"alpha.beta.1",
			"alpha.1",
			"alpha+beta",
			"alpha_beta",
			"alpha.",
			"alpha..",
			"beta",
			"1.0.0-alpha_beta",
			"-alpha.",
			"1.0.0-alpha..",
			"1.0.0-alpha..1",
			"1.0.0-alpha...1",
			"1.0.0-alpha....1",
			"1.0.0-alpha.....1",
			"1.0.0-alpha......1",
			"1.0.0-alpha.......1",
			"01.1.1",
			"1.01.1",
			"1.1.01",
			"1.2",
			"1.2.3.DEV",
			"1.2-SNAPSHOT",
			"1.2.31.2.3----RC-SNAPSHOT.12.09.1--..12+788",
			"1.2-RC-SNAPSHOT",
			"-1.0.3-gamma+b7718",
			"+justmeta",
			"9.8.7+meta+meta",
			"9.8.7-whatever+meta+meta",
			"99999999999999999999999.999999999999999999.99999999999999999----RC-SNAPSHOT.12.09.1--------------------------------..12"
		};
		
		for (int i = 0; i < invalid.GetLength(0); i += 1) {
			Assert.ThrowsException<SemanticVersion.InvalidSemanticVersionException>(() => _ = new SemanticVersion(invalid[i]));
		}

		Assert.IsTrue(new SemanticVersion(new BigInteger(0), new BigInteger(0), new BigInteger(0)).preRelease == string.Empty);
		Assert.IsTrue(new SemanticVersion(new BigInteger(0), new BigInteger(0), new BigInteger(0)).buildMetadata == string.Empty);
	}
	
	[TestMethod]
	public void CompareTo() {
		for (int i = 0; i < versions.GetLength(0); i += 1) {
			Assert.IsTrue(new SemanticVersion(versions[i,0]).CompareTo(new SemanticVersion(versions[i,1])) == -1);
			Assert.IsTrue(new SemanticVersion(versions[i,1]).CompareTo(new SemanticVersion(versions[i,0])) == 1);
		}
		string[] vs = new string[] {
			"0.0.259-pre.3",
			"0.0.0",
			"1.5.8+build.5",
			"4.7.4-23.54.wbt+3.tts.a"
		};
		foreach (string v in vs) {
			Assert.IsTrue(new SemanticVersion(v).CompareTo(new SemanticVersion(v)) == 0);
		}
	}

	[TestMethod]
	public void Clone() {
		SemanticVersion sv1 = new SemanticVersion("1.2.3-4+5");
		SemanticVersion sv2 = sv1.Clone();
		Assert.IsTrue(
			sv1.major == sv2.major &&
			sv1.minor == sv2.minor &&
			sv1.patch == sv2.patch &&
			sv1.preRelease == sv2.preRelease &&
			sv1.buildMetadata == sv2.buildMetadata &&
			sv1 == sv2
		);
	}

	[TestMethod]
	public void AsReadOnly() {
		SemanticVersion sv;
		ImmutableSemanticVersion immutable;
		for (int i = 0; i < versions.GetLength(0); i += 1) {
			for (int j = 0; j < versions.GetLength(1); j += 1) {
				sv = new SemanticVersion(versions[i, j]);
				immutable = new ImmutableSemanticVersion(versions[i, j]);
				Assert.IsTrue(sv.ToImmutable() == immutable);
			}
		}
	}

	[TestMethod]
	public void JsonConverter() {
		string str = "{\"semanticVersion\":\"1.2.3-4\\u002B5\",\"readOnlySemanticVersion\":\"5.4.3-2\\u002B1\"}";
		JsonConverterTest test = JsonSerializer.Deserialize<JsonConverterTest>(str, jsonSerializerOptions);
		Assert.IsTrue(test.semanticVersion == new SemanticVersion("1.2.3-4+5"), $"{test.semanticVersion}, {new SemanticVersion("1.2.3-4+5")}");
		Assert.IsTrue(test.readOnlySemanticVersion == new ImmutableSemanticVersion("5.4.3-2+1"), $"{test.readOnlySemanticVersion}, {new ImmutableSemanticVersion("5.4.3-2+1")}");
		Assert.IsTrue(JsonSerializer.Serialize(test, jsonSerializerOptions) == str, $"{JsonSerializer.Serialize(test, jsonSerializerOptions)}");
	}

	public class JsonConverterTest {
		public SemanticVersion semanticVersion { get; set; }
		public ImmutableSemanticVersion readOnlySemanticVersion { get; set; }
	}
}
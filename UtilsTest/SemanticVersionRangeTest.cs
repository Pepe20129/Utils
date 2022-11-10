using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Utils;

namespace UtilsTest;

[TestClass]
[ExcludeFromCodeCoverage]
public class SemanticVersionRangeTest {
	readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions {
		Converters = {
			new IParsableJsonCoverterFactory()
		}
	};

	[TestMethod]
	public void ExplicitInitialization() {
		SemanticVersionRange svr = new SemanticVersionRange(new SemanticVersion("0.0.0-0"), true, new SemanticVersion("4.7.2-735.fsfs+fdsf1234"), false, false);
		Assert.IsTrue(svr.min == new SemanticVersion("0.0.0-0"));
		Assert.IsTrue(svr.minActive == true);
		Assert.IsTrue(svr.max == new SemanticVersion("4.7.2-735.fsfs+fdsf1234"));
		Assert.IsTrue(svr.maxActive == false);
		Assert.IsTrue(svr.includePrereleases == false);

		Assert.IsTrue(new SemanticVersionRange(new SemanticVersion("1.0.0"), true, null, false, false) == new SemanticVersionRange(">=1.0.0"));
		Assert.IsTrue(new SemanticVersionRange(null, false, new SemanticVersion("69.420.0"), true, false) == new SemanticVersionRange("<69.420.0"));
		Assert.IsTrue(new SemanticVersionRange(null, false, null, false, false) == new SemanticVersionRange("*"));
		Assert.IsTrue(new SemanticVersionRange(new SemanticVersion("1.0.0"), true, new SemanticVersion("69.420.1-0"), true, false) == new SemanticVersionRange("1.0.0 - 69.420.0"));
	}

	[TestMethod]
	public void Incorrect() {
		string[] strings = new string[] {
			"1.2.3.4",
			"ewsrdtgs",
			"!1.7.9",
			"645.3547.743·$",
			"1.0.0-00",
			"01.5.4",
			"1.0.0 ",
			"1.0.0-",
			"1.0.0+",
			"~1.0.0+",
			"^1.0.0+",
			"<=1.0.0+",
			"<1.0.0+",
			">=1.0.0+",
			">1.0.0+",
			"=1.0.0+"
		};
		
		for (int i = 0; i < strings.Length; i += 1) {
			Assert.ThrowsException<SemanticVersionRange.InvalidSemanticVersionRangeException>(() => _ = new SemanticVersionRange(strings[i]));
		}
	}

	[TestMethod]
	public void Any() {
		SemanticVersionRange svr = new SemanticVersionRange("*");

		Assert.IsTrue(svr.min is null);
		Assert.IsFalse(svr.minActive);

		Assert.IsTrue(svr.max is null);
		Assert.IsFalse(svr.maxActive);


		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.6.0+fwsdfh")));

		svr.includePrereleases = false;
		Assert.IsFalse(svr.Includes(new SemanticVersion("5.0.0-gdchgf")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("5.0.0-gdchgf")));

		Assert.IsFalse(svr.Includes(null));
	}

	[TestMethod]
	public void CaretRange() {
		SemanticVersionRange svr = new SemanticVersionRange("^1.69.420");
		
		Assert.IsTrue(svr.min == new SemanticVersion("1.69.420"));
		Assert.IsTrue(svr.minActive);
		
		Assert.IsTrue(svr.max == new SemanticVersion("2.0.0-0"));
		Assert.IsTrue(svr.maxActive);

		Assert.IsFalse(svr.Includes(new SemanticVersion("1.18.1")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.69.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.635345.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.69.534535")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("0.34.543")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.69.0-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.69.999-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.0.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.0.0-gsdfsgb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.0.0+htdfgrth")));

		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange("^1.69");

		Assert.IsFalse(svr.Includes(new SemanticVersion("1.18.1")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.69.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.635345.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.69.534535")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("0.34.543")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.69.0-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.69.999-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.0.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.0.0-gsdfsgb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.0.0+htdfgrth")));

		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange("^1");

		Assert.IsTrue(svr.Includes(new SemanticVersion("1.18.1")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.69.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.635345.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.69.534535")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("0.34.543")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.69.0-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.69.999-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.0.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.0.0-gsdfsgb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.0.0+htdfgrth")));

		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange("^0");

		Assert.IsTrue(svr.Includes(new SemanticVersion("0.18.1")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.69.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.635345.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.69.534535")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("0.69.0-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.69.999-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0-gsdfsgb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0+htdfgrth")));

		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange("^0.0");

		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.534535")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("0.0.0-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.0.999-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.1.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0-gsdfsgb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0+htdfgrth")));

		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange("^0.1.69");

		Assert.IsTrue(svr.Includes(new SemanticVersion("0.1.534535")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.1.69")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("0.2.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.0.0-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.0.999-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.1.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0-gsdfsgb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0+htdfgrth")));

		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange("^0.0.0");

		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.1")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.534535")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("0.69.0-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.69.999-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.1.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0-gsdfsgb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0+htdfgrth")));

		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange("^0.x");

		Assert.IsTrue(svr.Includes(new SemanticVersion("0.18.1")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.69.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.635345.420")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.69.534535")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("0.69.0-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.69.999-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0-gsdfsgb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0+htdfgrth")));

		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange("^0.0.x");

		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.534535")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("0.0.0-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.0.999-pre.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.1.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0-gsdfsgb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("1.0.0+htdfgrth")));

		Assert.IsFalse(svr.Includes(null));
	}

	[TestMethod]
	public void Exact() {
		SemanticVersionRange svr = new SemanticVersionRange("=65.234.0");

		for (int i = 0; i < 3; i += 1) {
			Assert.IsTrue(svr.min == new SemanticVersion("65.234.0"));
			Assert.IsTrue(svr.minActive);

			Assert.IsTrue(svr.max == new SemanticVersion("65.234.1-0"));
			Assert.IsTrue(svr.maxActive);


			Assert.IsTrue(svr.Includes(new SemanticVersion("65.234.0")));
			Assert.IsTrue(svr.Includes(new SemanticVersion("65.234.0+rhfdv")));

			Assert.IsFalse(svr.Includes(new SemanticVersion("65.235.0")));
			Assert.IsFalse(svr.Includes(new SemanticVersion("65.234.0-gedge")));
			Assert.IsFalse(svr.Includes(new SemanticVersion("65.235.0-gedge")));
			Assert.IsFalse(svr.Includes(new SemanticVersion("65.235.0-1")));
			Assert.IsFalse(svr.Includes(new SemanticVersion("65.235.0-0")));
			Assert.IsFalse(svr.Includes(new SemanticVersion("65.235.0-0a")));
			Assert.IsFalse(svr.Includes(new SemanticVersion("65.235.0-a0")));
			Assert.IsFalse(svr.Includes(new SemanticVersion("65.235.0-a")));
			Assert.IsFalse(svr.Includes(new SemanticVersion("65.1245.0")));
			Assert.IsFalse(svr.Includes(new SemanticVersion("5.234.0")));
			Assert.IsFalse(svr.Includes(new SemanticVersion("65.234.24")));
			Assert.IsFalse(svr.Includes(null));

			if (i == 1) {
				svr = new SemanticVersionRange("65.234.0");
			}
			if (i == 2) {
				svr = new SemanticVersionRange("=65.234");
			}
		}
	}

	[TestMethod]
	public void GreaterOrEqual() {
		SemanticVersionRange svr = new SemanticVersionRange(">=2.4.3");

		Assert.IsTrue(svr.min == new SemanticVersion("2.4.3"));
		Assert.IsTrue(svr.minActive);

		Assert.IsTrue(svr.max is null);
		Assert.IsFalse(svr.maxActive);


		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.3+hefwas")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("5.4.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.7.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.5")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("1.4.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.3.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.2")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.3-gweyf")));


		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.4-gdsgse")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.4-gdsgse")));

		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange(">=2.4");
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.3+hefwas")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("5.4.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.7.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.8")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("1.4.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.3.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.3-gweyf")));
	}

	[TestMethod]
	public void Greater() {
		SemanticVersionRange svr = new SemanticVersionRange(">2.4.3");

		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.4-gweyf")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.4-gweyf")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.4+hefwas")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.4")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("5.4.4")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.7.4")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.5")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("1.4.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.3.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.2")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.3-gweyf")));
		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange(">2.4");

		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.4-gweyf")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.4-gweyf")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.4+hefwas")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.4")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("5.4.4")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.7.4")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.5")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("1.4.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.3.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.3-gweyf")));
		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange(">1.2.3-alpha.3");

		Assert.IsTrue(svr.Includes(new SemanticVersion("1.2.3-alpha.7")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.4.5-alpha.9")));
	}

	[TestMethod]
	public void LesserOrEqual() {
		SemanticVersionRange svr = new SemanticVersionRange("<=2.4.3");

		Assert.IsTrue(svr.min is null);
		Assert.IsFalse(svr.minActive);

		Assert.IsTrue(svr.max == new SemanticVersion("2.4.4-0"));
		Assert.IsTrue(svr.maxActive);


		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.3-gdsgse")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.3-gdsgse")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.3+hefwas")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.4.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.3.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.1")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.4.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.6.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.8")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.4-gweyf")));
		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange("<=2.4");

		Assert.IsFalse(svr.Includes(new SemanticVersion("2.3.3-gdsgse")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.3.3-gdsgse")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.0+hefwas")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.4.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.3.3")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.4.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.6.3")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.8")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.4-gweyf")));
		Assert.IsFalse(svr.Includes(null));
	}

	[TestMethod]
	public void Lesser() {
		SemanticVersionRange svr = new SemanticVersionRange("<2.4.3");

		Assert.IsTrue(svr.min is null);
		Assert.IsFalse(svr.minActive);

		Assert.IsTrue(svr.max == new SemanticVersion("2.4.3"));
		Assert.IsTrue(svr.maxActive);


		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.3-gdsgse")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.3-gdsgse")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.2+hefwas")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.2")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("1.4.2")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.3.2")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.4.1")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.4.2")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.6.2")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.8")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.4-gweyf")));
		Assert.IsFalse(svr.Includes(null));

		svr = new SemanticVersionRange("<2.4");

		Assert.IsFalse(svr.Includes(new SemanticVersion("2.3.99-gdsgse")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.3.99-gdsgse")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("1.4.2+hefwas")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("2.3.2")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.4.2")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.6.2")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.8")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.4.4-gweyf")));
		Assert.IsFalse(svr.Includes(null));
	}

	[TestMethod]
	public void XRange() {
		SemanticVersionRange svr = new SemanticVersionRange("3.6.x");


		Assert.IsFalse(svr.Includes(new SemanticVersion("2.9.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.5.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.0-gesdfgvb")));

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.8")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.7.0-0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.7.0")));


		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange("3.x");


		Assert.IsFalse(svr.Includes(new SemanticVersion("2.9.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.0.0-hdfgjhdfh")));

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.5.0")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.0-gesdfgvb")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.0-gesdfgvb")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.8")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("4.0.0-0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("4.0.0")));


		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange("3.x.x");


		Assert.IsFalse(svr.Includes(new SemanticVersion("2.9.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.0.0-hdfgjhdfh")));

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.5.0")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.0-gesdfgvb")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.0-gesdfgvb")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.8")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("4.0.0-0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("4.0.0")));


		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange("3.*.X-pre.1");


		Assert.IsFalse(svr.Includes(new SemanticVersion("2.9.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.0.0-hdfgjhdfh")));

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.5.0")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.0-gesdfgvb")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.0-gesdfgvb")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.8")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("4.0.0-0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("4.0.0")));


		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange("3.X.*-pre.1+build.1");


		Assert.IsFalse(svr.Includes(new SemanticVersion("2.9.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.0.0-hdfgjhdfh")));

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.5.0")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.0-gesdfgvb")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.0-gesdfgvb")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.8")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("4.0.0-0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("4.0.0")));


		Assert.IsFalse(svr.Includes(null));
	}

	[TestMethod]
	public void TildaRange() {
		SemanticVersionRange svr = new SemanticVersionRange("~3.6.4+build.5");

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.4")));

		svr.includePrereleases = true;
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.4-gesdfgvb")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.6-pre")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.7.0-0")));
		svr.includePrereleases = false;
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.4-gesdfgvb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.6-pre")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.7.0-0")));

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.8")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.7.0")));
		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange("~3.6");

		Assert.IsFalse(svr.Includes(new SemanticVersion("3.5.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.4")));

		svr.includePrereleases = true;
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.5.4-gesdfgvb")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.6-pre")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.7.0-0")));
		svr.includePrereleases = false;
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.5.4-gesdfgvb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.6-pre")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.7.0-0")));

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.8")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.7.0")));
		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange("~3");

		Assert.IsFalse(svr.Includes(new SemanticVersion("2.5.3")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.4")));

		svr.includePrereleases = true;
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.5.4-gesdfgvb")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.6-pre")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("4.7.0-0")));
		svr.includePrereleases = false;
		Assert.IsFalse(svr.Includes(new SemanticVersion("2.5.4-gesdfgvb")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("3.6.6-pre")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("4.7.0-0")));

		Assert.IsTrue(svr.Includes(new SemanticVersion("3.6.8")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("4.7.0")));
		Assert.IsFalse(svr.Includes(null));
	}

	[TestMethod]
	public void HyphenRange() {
		SemanticVersionRange svr = new SemanticVersionRange("0.0.0 - 45.76.12");
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.0.0-0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.1")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("5.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.4.0")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("34.643.12354-fgdfdfg.53534+gdgfs")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.12-0")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("34.643.12354-fgdfdfg.53534+gdgfs")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("45.76.12-0")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("45.76.12")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.13-0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.13")));
		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange("0.0.0-pre - 45.76.12-pre");
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.0.0-0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.1")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("5.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.4.0")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("34.643.12354-fgdfdfg.53534+gdgfs")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.13-0")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("34.643.12354-fgdfdfg.53534+gdgfs")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.13-0")));
		svr.includePrereleases = false;

		Assert.IsTrue(svr.Includes(new SemanticVersion("45.76.12-0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("45.76.12-pre")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.12-pre.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.12")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.13")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("100.0.0")));
		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange("0 - 45.76");
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.0.0-0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.1")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("5.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.4.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("45.76.0")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("34.643.12354-fgdfdfg.53534+gdgfs")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.0-0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.1-0")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("34.643.12354-fgdfdfg.53534+gdgfs")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("45.76.0-0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.1-0")));
		svr.includePrereleases = false;

		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.0-pre.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.76.1")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("45.77.0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("100.0.0")));
		Assert.IsFalse(svr.Includes(null));


		svr = new SemanticVersionRange("0 - 45");
		Assert.IsFalse(svr.Includes(new SemanticVersion("0.0.0-0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.0.1")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("5.0.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("0.4.0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("45.0.0")));

		Assert.IsFalse(svr.Includes(new SemanticVersion("34.643.12354-fgdfdfg.53534+gdgfs")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("44.76.0-0")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("44.76.1-0")));
		svr.includePrereleases = true;
		Assert.IsTrue(svr.Includes(new SemanticVersion("34.643.12354-fgdfdfg.53534+gdgfs")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("44.76.0-0")));
		Assert.IsTrue(svr.Includes(new SemanticVersion("44.76.1-0")));
		svr.includePrereleases = false;

		Assert.IsFalse(svr.Includes(new SemanticVersion("45.69.420")));
		Assert.IsFalse(svr.Includes(new SemanticVersion("100.0.0")));
		Assert.IsFalse(svr.Includes(null));
	}

	[TestMethod]
	public void Equals() {
		SemanticVersionRange svr1;
		SemanticVersionRange svr2;
		SemanticVersionRange svr3;
		string[] strings = new string[] { "*", "0.0.0", "^0.0.0", "~0.0.0", "0.0.x", "0.0.0 - 0.0.0" };
		foreach (string str in strings) {
			svr1 = new SemanticVersionRange(str);
			svr2 = new SemanticVersionRange(str);
			svr3 = new SemanticVersionRange("1.0.0");
			#pragma warning disable CS1718 // Comparación hecha a la misma variable
			Assert.IsTrue(svr1.Equals(svr1));
			Assert.IsTrue(svr1 == svr1);
			Assert.IsFalse(svr1 != svr1);
			Assert.IsTrue(svr1.Equals(svr2));
			Assert.IsTrue(svr1 == svr2);
			Assert.IsFalse(svr1 != svr2);
			Assert.IsTrue(svr2.Equals(svr1));
			Assert.IsTrue(svr2 == svr1);
			Assert.IsFalse(svr2 != svr1);
			Assert.IsTrue(svr2.Equals(svr2));
			Assert.IsTrue(svr2 == svr2);
			Assert.IsFalse(svr2 != svr2);
			Assert.IsFalse(svr1.Equals(svr3));
			Assert.IsFalse(svr1 == svr3);
			Assert.IsTrue(svr1 != svr3);
			Assert.IsFalse(svr2.Equals(svr3));
			Assert.IsFalse(svr2 == svr3);
			Assert.IsTrue(svr2 != svr3);
			Assert.IsFalse(svr3.Equals(svr1));
			Assert.IsFalse(svr3 == svr1);
			Assert.IsTrue(svr3 != svr1);
			Assert.IsFalse(svr3.Equals(svr2));
			Assert.IsFalse(svr3 == svr2);
			Assert.IsTrue(svr3 != svr2);

			Assert.IsTrue(svr1.Equals((object)svr1));
			Assert.IsTrue(svr1.Equals((object)svr2));
			Assert.IsTrue(svr2.Equals((object)svr1));
			Assert.IsTrue(svr2.Equals((object)svr2));
			Assert.IsFalse(svr1.Equals((object)svr3));
			Assert.IsFalse(svr2.Equals((object)svr3));
			Assert.IsFalse(svr3.Equals((object)svr1));
			Assert.IsFalse(svr3.Equals((object)svr2));

			Assert.IsFalse(svr1.Equals("a"));
			Assert.IsFalse(svr1.Equals(0));
			Assert.IsFalse(svr1.Equals(new SemanticVersion("0.0.0-0")));
			Assert.IsFalse(svr2.Equals("a"));
			Assert.IsFalse(svr2.Equals(0));
			Assert.IsFalse(svr2.Equals(new SemanticVersion("0.0.0-0")));
			Assert.IsFalse(svr3.Equals("a"));
			Assert.IsFalse(svr3.Equals(0));
			Assert.IsFalse(svr3.Equals(new SemanticVersion("0.0.0-0")));

			Assert.IsTrue(svr1.Equals(svr1.Clone()));
			Assert.IsTrue(svr1 == svr1.Clone());
			Assert.IsFalse(svr1 != svr1.Clone());
			Assert.IsTrue(svr2.Equals(svr2.Clone()));
			Assert.IsTrue(svr2 == svr2.Clone());
			Assert.IsFalse(svr2 != svr2.Clone());
			Assert.IsTrue(svr3.Equals(svr3.Clone()));
			Assert.IsTrue(svr3 == svr3.Clone());
			Assert.IsFalse(svr3 != svr3.Clone());

			Assert.IsFalse(null == svr1);
			Assert.IsFalse(null == svr2);
			Assert.IsFalse(null == svr3);
			Assert.IsFalse(svr1 == null);
			Assert.IsFalse(svr2 == null);
			Assert.IsFalse(svr3 == null);
			#pragma warning restore CS1718 // Comparación hecha a la misma variable
		}
	}

	[TestMethod]
	public void Partial() {
		Assert.IsTrue(new SemanticVersionRange("24.69").Equals(new SemanticVersionRange("24.69.0")));
		Assert.IsTrue(new SemanticVersionRange("24").Equals(new SemanticVersionRange("24.0.0")));
		Assert.IsTrue(new SemanticVersionRange("24.420").Equals(new SemanticVersionRange("24.420.0")));
		Assert.IsTrue(new SemanticVersionRange("1.2").Equals(new SemanticVersionRange("1.2.0")));
		Assert.IsTrue(new SemanticVersionRange("1").Equals(new SemanticVersionRange("1.0.0")));
	}

	[TestMethod]
	public void TestToString() {
		string[] strings = new string[] { "*", "0.0.0", "^0.0.0", "~0.0.0", "0.0.x", "0.0.0 - 0.0.0", string.Empty };
		foreach (string str in strings) {
			Assert.IsTrue(new SemanticVersionRange(str).ToString() == str);
		}
		Assert.IsTrue(new SemanticVersionRange(new SemanticVersion("1.0.0"), true, new SemanticVersion("69.420.0"), false, false).ToString() == ">=1.0.0");
		Assert.IsTrue(new SemanticVersionRange(new SemanticVersion("1.0.0"), false, new SemanticVersion("69.420.0"), true, false).ToString() == "<69.420.0");
		Assert.IsTrue(new SemanticVersionRange(new SemanticVersion("1.0.0"), false, new SemanticVersion("69.420.0"), false, false).ToString() == "*");
		Assert.IsTrue(new SemanticVersionRange(new SemanticVersion("1.0.0"), true, new SemanticVersion("69.420.1-0"), true, false).ToString() == "1.0.0 - 69.420.0");
		try {
			_ = new SemanticVersionRange(new SemanticVersion("0.0.0"), true, new SemanticVersion("0.0.0"), true, false).ToString();
			Assert.Fail();
		} catch (NotImplementedException) {}
	}

	[TestMethod]
	public void TestGetHashCode() {
		string[] strings = new string[] { "*", "0.0.0", "^0.0.0", "~0.0.0", "0.0.x", "0.0.0 - 0.0.0" };
		foreach (string str in strings) {
			Assert.IsTrue(new SemanticVersionRange(str).GetHashCode() == str.GetHashCode());
		}
	}

	[TestMethod]
	public void JsonConverter() {
		string str = "{\"semanticVersionRange\":\"\\u003E=1.2.3-4\\u002B5\"}";
		JsonConverterTest test = JsonSerializer.Deserialize<JsonConverterTest>(str, jsonSerializerOptions);
		Assert.IsTrue(test.semanticVersionRange == new SemanticVersionRange(">=1.2.3-4+5"));
		Assert.IsTrue(JsonSerializer.Serialize(test, jsonSerializerOptions) == str, $"{JsonSerializer.Serialize(test, jsonSerializerOptions)}");
	}

	public class JsonConverterTest {
		public SemanticVersionRange semanticVersionRange { get; set; }
	}
}
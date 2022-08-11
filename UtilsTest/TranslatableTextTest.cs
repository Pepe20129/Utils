using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using Utils;

namespace UtilsTest;

[TestClass]
[ExcludeFromCodeCoverage]
public class TranslatableTextTest {
	[TestMethod]
	public void Main() {
		/*
		 {
		     "test": "success",
		     "test2": {
		         "a": "b",
		         "b": [
		             "1",
		             "2"
		         ]
		     }
		 }
		 */
		TranslatableText.TranslatableTextSettings settings = new TranslatableText.TranslatableTextSettings {
			data = JsonSerializer.Deserialize<JsonElement>("{\"test\":\"success\", \"test2\": {\"a\": \"b\", \"b\": [\"1\", \"2\"]}}"),
			random = new Random(0)
		};
		TranslatableText.SetSettings(settings);
		Assert.IsTrue(TranslatableText.GetSettings() == settings);
		Assert.IsFalse(new TranslatableText("test").ToString() == new TranslatableText("test").ToStringAsAssembly(Assembly.GetAssembly(typeof(TranslatableText))));

		Assert.IsTrue(new TranslatableText("test") == "success");
		Assert.IsTrue(new TranslatableText("test2.a") == "b");
		Assert.IsTrue(new TranslatableText("test2.b") == "2");
		Assert.IsTrue(new TranslatableText("test2.b") == "2");
		Assert.IsTrue(new TranslatableText("test2.b") == "2");
		Assert.IsTrue(new TranslatableText("test2.b") == "1");
		Assert.IsTrue(new TranslatableText("test2.b") == "1");

		settings = settings with {
			subKeySplitCharacter = ',',
			random = new Random(0)
		};
		TranslatableText.SetSettings(settings);
		Assert.IsTrue(new TranslatableText("test") == "success");
		Assert.IsTrue(new TranslatableText("test2,a") == "b");
		Assert.IsTrue(new TranslatableText("test2,b") == "2");
		Assert.IsTrue(new TranslatableText("test2,b") == "2");
		Assert.IsTrue(new TranslatableText("test2,b") == "2");
		Assert.IsTrue(new TranslatableText("test2,b") == "1");
		Assert.IsTrue(new TranslatableText("test2,b") == "1");

		settings = settings with {
			subKeySplitCharacter = '.'
		};
		TranslatableText.SetSettings(settings);
		Assert.IsTrue("b".GetHashCode() == new TranslatableText("test2.a").GetHashCode());
		Assert.IsTrue(new TranslatableText("test2.a").Equals(new TranslatableText("test2.a")));
		Assert.IsTrue(new TranslatableText("test2.a") == new TranslatableText("test2.a"));
		Assert.IsFalse(new TranslatableText("test2.a") != new TranslatableText("test2.a"));
		Assert.IsTrue(new TranslatableText("test2.a").Equals("b"));
		Assert.IsFalse(new TranslatableText("test2.a").Equals(3));
		Assert.IsFalse(new TranslatableText("test2.a") == (TranslatableText)null);
		Assert.IsFalse((TranslatableText)null == new TranslatableText("test2.a"));
		Assert.IsTrue((TranslatableText)null == (TranslatableText)null);

		Assert.ThrowsException<TranslatableText.TranslatableKeyNotFoundException>(() => _ = new TranslatableText("test2"));
		Assert.ThrowsException<TranslatableText.TranslatableKeyNotFoundException>(() => _ = new TranslatableText("test3"));
		Assert.ThrowsException<TranslatableText.TranslatableKeyNotFoundException>(() => _ = new TranslatableText("test2.a.b"));
		Assert.ThrowsException<TranslatableText.TranslatableKeyNotFoundException>(() => _ = new TranslatableText(string.Empty));
		Assert.ThrowsException<ArgumentNullException>(() => _ = new TranslatableText(null));
		Assert.ThrowsException<ArgumentNullException>(() => _ = new TranslatableText("test", (Assembly)null));

		settings = settings with {
			error = false
		};
		TranslatableText.SetSettings(settings);
		try {
			_ = new TranslatableText("test2");
		} catch {
			Assert.Fail();
		}

		try {
			_ = new TranslatableText("test3");
		} catch {
			Assert.Fail();
		}

		try {
			_ = new TranslatableText("test2.a.b");
		} catch {
			Assert.Fail();
		}

		try {
			_ = new TranslatableText(string.Empty);
		} catch {
			Assert.Fail();
		}
	}

	[TestMethod]
	public void Clone() {
		TranslatableText.TranslatableTextSettings settings = new TranslatableText.TranslatableTextSettings {
			data = JsonSerializer.Deserialize<JsonElement>("{\"test\":\"success\"}")
		};
		TranslatableText.SetSettings(settings);
		TranslatableText translatableText = new TranslatableText("test");
		Assert.IsTrue(translatableText.Equals(translatableText.Clone()));
	}
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static Utils.Utils;

namespace UtilsTest;

[TestClass]
[ExcludeFromCodeCoverage]
public class UtilsTest {
	[TestMethod]
	public void BitByte() {
		Assert.IsTrue(Enumerable.SequenceEqual(ByteToBits(255), new bool[] { true, true, true, true, true, true, true, true }));
		Assert.IsTrue(Enumerable.SequenceEqual(ByteToBits(127), new bool[] { false, true, true, true, true, true, true, true }));
		Assert.IsTrue(Enumerable.SequenceEqual(ByteToBits(69), new bool[] { false, true, false, false, false, true, false, true }));
		Assert.IsTrue(Enumerable.SequenceEqual(ByteToBits(24), new bool[] { false, false, false, true, true, false, false, false }));
		Assert.IsTrue(Enumerable.SequenceEqual(ByteToBits(0), new bool[] { false, false, false, false, false, false, false, false }));

		Assert.AreEqual(255, BitsToByte(new bool[] { true, true, true, true, true, true, true, true }));
		Assert.AreEqual(127, BitsToByte(new bool[] { false, true, true, true, true, true, true, true }));
		Assert.AreEqual(69, BitsToByte(new bool[] { false, true, false, false, false, true, false, true }));
		Assert.AreEqual(24, BitsToByte(new bool[] { false, false, false, true, true, false, false, false }));
		Assert.AreEqual(0, BitsToByte(new bool[] { false, false, false, false, false, false, false, false }));
	}

	[TestMethod]
	public void Shuffle() {
		List<int> list1 = new List<int>();
		List<int> list2 = new List<int>();
		Random random = new Random();
		for (int i = 0; i < 100; i += 1) {
			int r = random.Next(10000);
			list1.Add(r);
			list2.Add(r);
		}
		int r2 = random.Next(10000);
		Shuffle<int>(list1, new Random(r2));
		Shuffle<int>(list2, new Random(r2));
		Assert.IsTrue(Enumerable.SequenceEqual(list1, list2));

		//checks that it can't be shuffled back into itself
		for (int i = 0; i < 1000; i += 1) {
			List<int> small1 = new List<int> {1, 2};
			Shuffle<int>(small1);
			Assert.IsFalse(Enumerable.SequenceEqual(small1, new List<int> {1, 2}));

			List<int> small2 = new List<int> {1, 2, 3};
			Shuffle<int>(small2);
			Assert.IsFalse(Enumerable.SequenceEqual(small2, new List<int> {1, 2, 3}));
		}
	}

	[TestMethod]
	public void ArgsParser() {
		string[] args = new string[] { "--test", "-test2", "result", "-t", "-e", "e-", "--e" };
		Dictionary<string, string> arguments = Utils.Utils.ArgsParser(args);
		Dictionary<string, string> expectedArguments = new Dictionary<string, string> { {"test", "true"}, {"test2", "result"}, {"t", "-e"}, { "e", "true" } };
		foreach (KeyValuePair<string, string> keyValuePair in arguments) {
			if (!expectedArguments.ContainsKey(keyValuePair.Key))
				Assert.Fail();
			if (arguments[keyValuePair.Key] != expectedArguments[keyValuePair.Key])
				Assert.Fail();
		}

		foreach (KeyValuePair<string, string> keyValuePair in expectedArguments) {
			if (!arguments.ContainsKey(keyValuePair.Key))
				Assert.Fail();
			if (arguments[keyValuePair.Key] != expectedArguments[keyValuePair.Key])
				Assert.Fail();
		}


		Dictionary<string, string> empty1 = Utils.Utils.ArgsParser(Array.Empty<string>());
		Dictionary<string, string> empty2 = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> keyValuePair in empty1) {
			if (!empty2.ContainsKey(keyValuePair.Key))
				Assert.Fail();
			if (empty1[keyValuePair.Key] != empty2[keyValuePair.Key])
				Assert.Fail();
		}

		foreach (KeyValuePair<string, string> keyValuePair in empty2) {
			if (!empty1.ContainsKey(keyValuePair.Key))
				Assert.Fail();
			if (empty1[keyValuePair.Key] != empty2[keyValuePair.Key])
				Assert.Fail();
		}
	}

	[TestMethod]
	public void GetDisplayLength() {
		Assert.IsTrue(Utils.Utils.GetDisplayLength("") == 0);
		Assert.IsTrue(Utils.Utils.GetDisplayLength("test") == 4);
		Assert.IsTrue(Utils.Utils.GetDisplayLength("§ctest§r") == 4);
		Assert.IsTrue(Utils.Utils.GetDisplayLength("\\§ctest\\§r") == 8);
	}

	[TestMethod]
	public void Cryptography() {
		string message = "This is a cryptography test where this message gets encrypted with a random key and then decrypted with that same key.";

		//generate a random key
		string keyString = string.Empty;
		Random random = new Random();
		for (int i = 0; i < 64; i += 1) {
			keyString += "0123456789abcdef"[random.Next(16)];
		}

		byte[] key = HexStringToByteArray(keyString);
		byte[] encryptedData = AesEncrypt(message, key);
		string decryptedData = AesDecrypt(encryptedData, key);
		Assert.IsTrue(message == decryptedData);

		try {
			_ = AesEncrypt(message, null);
			Assert.Fail();
		} catch (ArgumentNullException) {}

		try {
			_ = AesDecrypt(encryptedData, null);
			Assert.Fail();
		} catch (ArgumentNullException) {}

		try {
			_ = AesEncrypt(null, key);
			Assert.Fail();
		} catch (ArgumentNullException) {}

		try {
			_ = AesDecrypt(null, key);
			Assert.Fail();
		} catch (ArgumentNullException) {}

		try {
			_ = AesEncrypt(message, key[..^1]);
			Assert.Fail();
		} catch (ArgumentException) {}

		try {
			_ = AesDecrypt(encryptedData, key[..^1]);
			Assert.Fail();
		} catch (ArgumentException) {}
	}
	
	[TestMethod]
	public void GetHashString() {
		Assert.IsTrue(Utils.Utils.GetHashString("a") == "CA978112CA1BBDCAFAC231B39A23DC4DA786EFF8147C4E72B9807785AFEE48BB");
		Assert.IsTrue(Utils.Utils.GetHashString("b") == "3E23E8160039594A33894F6564E1B1348BBD7A0088D42C4ACB73EEAED59C009D");
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Utils;

/// <summary>
/// A class with multiple utility methods
/// </summary>
public static class Utils {

	#region Print

	/*
	 * These are excluded from code coverage because they are visual and idk how to test them
	 */

	/// <summary>
	/// Takes an IEnumerable and prints every entry in it separetly
	/// </summary>
	/// <param name="enumerable">The IList to print</param>
	/// <param name="format">The string format to print with, defaults to "{0}"</param>
	/// <param name="stringConverter">A function that converts a <typeparamref name="T"/> into a <see cref="string"/>, defaults to <see cref="object.ToString"/></param>
	[ExcludeFromCodeCoverage]
	public static void PrintIEnumerable<T>(IEnumerable<T> enumerable, string format = "{0}", Func<T, string> stringConverter = null) {
		stringConverter ??= [ExcludeFromCodeCoverage] (t) => t.ToString();
		foreach (T t in enumerable) {
			if (t is null)
				PrintColoredText(string.Format(format, "null"));
			else
				PrintColoredText(string.Format(format, stringConverter.Invoke(t)));
		}
	}

	/// <summary>
	/// Takes an ITuple and prints every entry in it separetly
	/// </summary>
	/// <param name="tuple">The ITuple to print</param>
	/// <param name="format">The string format to print with</param>
	[ExcludeFromCodeCoverage]
	public static void PrintITuple(ITuple tuple, string format = "{0}") {
		PrintIEnumerable(tuple.GetType().GetProperties().Select([ExcludeFromCodeCoverage] (p) => p.GetValue(tuple)), format);
	}

	/// <summary>
	/// The default color for the foreground. Used in PrintColoredText
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static ConsoleColor printColoredTextForegroundDefault { get; set; } = ConsoleColor.Gray;

	/// <summary>
	/// The default color for the background. Used in PrintColoredText
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static ConsoleColor printColoredTextBackgroundDefault { get; set; } = ConsoleColor.Black;

	/// <summary>
	/// Prints the object's ToString() inputted with colors signaled by '§'
	/// <br/><br/>
	/// [0-f] sets foreground color, [g-w] (excluding r) sets background color
	/// <br/>
	/// 'x' resets foreground color, 'y' resets background color, and 'r' resets both
	/// </summary>
	/// <param name="raw">The object to print with colors</param>
	[ExcludeFromCodeCoverage]
	public static void PrintColoredText(object raw) {
		if (raw is null)
			throw new ArgumentNullException(nameof(raw));

		string rawString = raw.ToString();
		if (!rawString.Contains('§')) {
			Console.WriteLine(rawString);
			return;
		}
		char[] chars = rawString.ToCharArray();
		bool section = false;
		bool omit = false;
		for (int i = 0; i < chars.Length; i += 1) {
			char c = chars[i];

			//if the previous character was a '§', we have look at this character to determine the new color
			if (section) {
				section = false;
				Console.ForegroundColor = c switch {
					'0' => ConsoleColor.Black,
					'1' => ConsoleColor.DarkBlue,
					'2' => ConsoleColor.DarkGreen,
					'3' => ConsoleColor.DarkCyan,
					'4' => ConsoleColor.DarkRed,
					'5' => ConsoleColor.DarkMagenta,
					'6' => ConsoleColor.DarkYellow,
					'7' => ConsoleColor.Gray,
					'8' => ConsoleColor.DarkGray,
					'9' => ConsoleColor.Blue,
					'a' => ConsoleColor.Green,
					'b' => ConsoleColor.Cyan,
					'c' => ConsoleColor.Red,
					'd' => ConsoleColor.Magenta,
					'e' => ConsoleColor.Yellow,
					'f' => ConsoleColor.White,

					'r' => printColoredTextForegroundDefault,
					'x' => printColoredTextForegroundDefault,
					_ => Console.ForegroundColor
				};

				Console.BackgroundColor = c switch {
					'g' => ConsoleColor.Black,
					'h' => ConsoleColor.DarkBlue,
					'i' => ConsoleColor.DarkGreen,
					'j' => ConsoleColor.DarkCyan,
					'k' => ConsoleColor.DarkRed,
					'l' => ConsoleColor.DarkMagenta,
					'm' => ConsoleColor.DarkYellow,
					'n' => ConsoleColor.Gray,
					'o' => ConsoleColor.DarkGray,
					'p' => ConsoleColor.Blue,
					'q' => ConsoleColor.Green,
					's' => ConsoleColor.Cyan,
					't' => ConsoleColor.Red,
					'u' => ConsoleColor.Magenta,
					'v' => ConsoleColor.Yellow,
					'w' => ConsoleColor.White,

					'r' => printColoredTextBackgroundDefault,
					'y' => printColoredTextBackgroundDefault,
					_ => Console.BackgroundColor
				};
				continue;
			}

			//if this character is a '\' and the next character is a '§', we omit it
			if (c == '\\' && chars.Length > i + 1 && chars[i + 1] == '§') {
				omit = true;
				continue;
			}

			//only signal that there is a '§' if it wasn't omitted
			if (!omit && c == '§') {
				section = true;
				continue;
			}

			omit = false;
			Console.Write(c);
		}
		Console.WriteLine();
	}

	#endregion

	/// <summary>
	/// Gets a <see cref="ReadOnlySemanticVersion"/> representing the current version of this library
	/// </summary>
	/// <returns>
	/// A <see cref="ReadOnlySemanticVersion"/> representing the current version of this library
	/// </returns>
	public static ReadOnlySemanticVersion GetUtilsVersion() => new ReadOnlySemanticVersion("0.1.0");
	
	/// <summary>
	/// Gets the display length that a string would have if used in <see cref="PrintColoredText(object)"/>
	/// </summary>
	/// <param name="str">The string to check the display length</param>
	/// <returns>The display length of <paramref name="str"/></returns>
	public static int GetDisplayLength(string str) {
		if (str is null)
			throw new ArgumentNullException(nameof(str));
		
		if (!str.Contains('§'))
			return str.Length;
		
		int count = 0;
		char[] chars = str.ToCharArray();
		bool section = false;
		bool omit = false;
		for (int i = 0; i < chars.Length; i += 1) {
			char c = chars[i];

			//if the previous character was a '§', we have look at this character to determine the new color
			if (section) {
				section = false;
				continue;
			}

			//if this character is a '\' and the next character is a '§', we omit it
			if (c == '\\' && chars.Length > i + 1 && chars[i + 1] == '§') {
				omit = true;
				continue;
			}

			//only signal that there is a '§' if it wasn't omitted
			if (!omit && c == '§') {
				section = true;
				continue;
			}

			omit = false;
			count += 1;
		}
		return count;
	}

	/// <summary>
	/// Parses a string array of arguments into a dictionary containing those arguments
	/// </summary>
	/// <param name="args">The args to parse</param>
	/// <returns>A dictionary with the args parsed</returns>
	public static Dictionary<string, string> ArgsParser(string[] args) {
		if (args is null)
			throw new ArgumentNullException(nameof(args));

		Dictionary<string, string> dict = new Dictionary<string, string>();
		if (args.Length == 0)
			return dict;

		for (int i = 0; i < args.Length; i += 1) {
			if (args[i][0] == '-' && args[i][1] == '-') {//if it starts with "--"
				dict.Add(args[i][2..], "true");
			} else if (args[i][0] == '-') {//if it starts with "-"
				dict.Add(args[i][1..], args[i + 1]);
				//add 1 to the count so the we don't check the value again
				i += 1;
			}
		}
		return dict;
	}

	/// <summary>
	/// Takes an IList and shuffles it
	/// </summary>
	/// <typeparam name="T">The type of the elements on the IList</typeparam>
	/// <param name="list">The IList to shuffle</param>
	/// <param name="random">An instance of System.Random to use</param>
	public static void Shuffle<T>(IList<T> list, Random random = null) {
		if (list is null)
			throw new ArgumentNullException(nameof(list));

		random ??= new Random();
		for (int i = 1; i < list.Count; i += 1) {
			int n = random.Next(i);
			(list[n], list[i]) = (list[i], list[n]);
		}
	}

	/// <summary>
	/// Converts the first 8 bools in a bool array to a byte
	/// </summary>
	/// <param name="bits">The bits to convert to a byte</param>
	/// <returns>A byte made of the bools</returns>
	public static byte BitsToByte(bool[] bits) {
		return (byte)(
			(bits[0] ? 1 : 0) * 128 +
			(bits[1] ? 1 : 0) * 64 +
			(bits[2] ? 1 : 0) * 32 +
			(bits[3] ? 1 : 0) * 16 +
			(bits[4] ? 1 : 0) * 8 +
			(bits[5] ? 1 : 0) * 4 +
			(bits[6] ? 1 : 0) * 2 +
			(bits[7] ? 1 : 0)
		);
	}

	/// <summary>
	/// Converts a byte in a 8-length bool array
	/// </summary>
	/// <param name="b">The byte to convert to a bool array</param>
	/// <returns>A bool array made of the byte</returns>
	public static bool[] ByteToBits(byte b) {
		bool[] result = new bool[8];
		byte power;
		for (int i = 0; i < 8; i += 1) {
			power = (byte)Math.Pow(2, 7 - i);
			if (b >= power) {
				result[i] = true;
				b -= power;
			}
		}
		return result;
	}

	/// <summary>
	/// Converts a hex string to a <see cref="byte"/> <see cref="Array"/>
	/// </summary>
	/// <param name="hex">The hex string to convert</param>
	/// <returns>
	/// A <see cref="byte"/> <see cref="Array"/> representing <paramref name="hex"/>
	/// </returns>
	public static byte[] HexStringToByteArray(string hex) {
		return Enumerable.Range(0, hex.Length)
						 .Where(x => x % 2 == 0)
						 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
						 .ToArray();
	}

	//this is hardcoded, doesn't really matter, it's practically just another key
	private static readonly byte[] iv = new byte[16] { 30, 188, 169, 243, 90, 122, 131, 243, 83, 72, 206, 153, 144, 119, 198, 91 };

	/// <summary>
	/// Encrypts a string of text with the AES cryptographic algorithm
	/// </summary>
	/// <param name="plainText">The text to encrypt</param>
	/// <param name="key">The key to use for the encryption</param>
	/// <returns>
	/// The data from <paramref name="plainText"/>, encrypted
	/// </returns>
	public static byte[] AesEncrypt(string plainText, byte[] key) {
		if (plainText == null || plainText.Length <= 0)
			throw new ArgumentNullException(nameof(plainText));
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (key.Length != 32)
			throw new ArgumentException(nameof(key) + " has to have a length of 32");
		byte[] encrypted;

		// Create an Aes object
		// with the specified key and IV.
		using (Aes aesAlg = Aes.Create()) {
			aesAlg.Key = key;
			aesAlg.IV = iv;

			// Create an encryptor to perform the stream transform.
			ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for encryption.
			using (MemoryStream msEncrypt = new MemoryStream()) {
				using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
					using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
						//Write all data to the stream.
						swEncrypt.Write(plainText);
					}
					encrypted = msEncrypt.ToArray();
				}
			}
		}

		// Return the encrypted bytes from the memory stream.
		return encrypted;
	}

	/// <summary>
	/// Decrypts a <see cref="byte"/> <see cref="Array"/> with the AES cryptographic algorithm
	/// </summary>
	/// <param name="encryptedText">The data to decrypt</param>
	/// <param name="key">The key to use for the decryption</param>
	/// <returns>
	/// The data from <paramref name="encryptedText"/>, decrypted
	/// </returns>
	public static string AesDecrypt(byte[] encryptedText, byte[] key) {
		if (encryptedText == null || encryptedText.Length <= 0)
			throw new ArgumentNullException(nameof(encryptedText));
		if (key == null)
			throw new ArgumentNullException(nameof(key));
		if (key.Length != 32)
			throw new ArgumentException(nameof(key) + " has to have a length of 32");

		// Declare the string used to hold
		// the decrypted text.
		string plaintext = null;

		// Create an Aes object
		// with the specified key and IV.
		using (Aes aesAlg = Aes.Create()) {
			aesAlg.Key = key;
			aesAlg.IV = iv;

			// Create a decryptor to perform the stream transform.
			ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for decryption.
			using (MemoryStream msDecrypt = new MemoryStream(encryptedText)) {
				using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
					using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {

						// Read the decrypted bytes from the decrypting stream
						// and place them in a string.
						plaintext = srDecrypt.ReadToEnd();
					}
				}
			}
		}

		return plaintext;
	}

	/// <summary>
	/// Hashes the <paramref name="inputString"/> with <see cref="SHA256"/>
	/// </summary>
	/// <param name="inputString">The string to hash with <see cref="SHA256"/></param>
	/// <returns>The <see cref="SHA256"/>'d hashed version of the <paramref name="inputString"/></returns>
	public static string GetHashString(string inputString) {
		StringBuilder sb = new StringBuilder();
		byte[] array;
		using (HashAlgorithm algorithm = SHA256.Create()) {
			array = algorithm.ComputeHash(Encoding.Default.GetBytes(inputString));
		}
		foreach (byte b in array)
			sb.Append(b.ToString("X2"));
		return sb.ToString();
	}
}
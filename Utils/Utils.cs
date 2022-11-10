using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
	/// Takes an <see cref="IEnumerable{T}"/> and prints every entry in it separetly
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> of the items in the <see cref="IEnumerable{T}"/></typeparam>
	/// <param name="enumerable">The <see cref="IEnumerable{T}"/> to print</param>
	/// <param name="format">The <see cref="string"/> format to print with</param>
	/// <param name="stringConverter">A function that converts a <typeparamref name="T"/> into a <see cref="string"/>, defaults to <see cref="object.ToString"/></param>
	[ExcludeFromCodeCoverage]
	public static void PrintIEnumerable<T>(IEnumerable<T> enumerable, string format = "{0}", Func<T, string?>? stringConverter = null) {
		ArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
		ArgumentNullException.ThrowIfNull(format, nameof(format));
		stringConverter ??= [ExcludeFromCodeCoverage] (t) => t?.ToString();
		foreach (T t in enumerable) {
			if (t is null)
				PrintColoredText(string.Format(format, "null"));
			else
				PrintColoredText(string.Format(format, stringConverter.Invoke(t) ?? "null"));
		}
	}

	/// <summary>
	/// Takes an <see cref="ITuple"/> and prints every entry in it separetly
	/// </summary>
	/// <param name="tuple">The <see cref="ITuple"/> to print</param>
	/// <param name="format">The <see cref="string"/> format to print with</param>
	[ExcludeFromCodeCoverage]
	public static void PrintITuple(ITuple tuple, string format = "{0}") {
		ArgumentNullException.ThrowIfNull(tuple, nameof(tuple));
		ArgumentNullException.ThrowIfNull(format, nameof(format));
		PrintIEnumerable(tuple.GetType().GetProperties().Select([ExcludeFromCodeCoverage] (p) => p.GetValue(tuple)), format);
	}

	/// <summary>
	/// The default color for the foreground. Used in <see cref="PrintColoredText(object)"/>
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static ConsoleColor printColoredTextForegroundDefault { get; set; } = ConsoleColor.Gray;

	/// <summary>
	/// The default color for the background. Used in <see cref="PrintColoredText(object)"/>
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static ConsoleColor printColoredTextBackgroundDefault { get; set; } = ConsoleColor.Black;

	/// <summary>
	/// Prints the <see cref="object"/>'s <see cref="object.ToString"/> inputted with colors signaled by '§'
	/// <br/><br/>
	/// [0-f] sets foreground color, [g-w] (excluding r) sets background color
	/// <br/>
	/// 'x' resets foreground color, 'y' resets background color, and 'r' resets both
	/// </summary>
	/// <param name="raw">The <see cref="object"/> to print with colors</param>
	[ExcludeFromCodeCoverage]
	public static void PrintColoredText(object raw) {
		ArgumentNullException.ThrowIfNull(raw, nameof(raw));

		string rawString = raw.ToString() ?? throw new ArgumentException($"{nameof(raw)}.ToString() returned null", nameof(raw));
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
	/// Gets an <see cref="ImmutableSemanticVersion"/> representing the current version of this library
	/// </summary>
	/// <returns>
	/// An <see cref="ImmutableSemanticVersion"/> representing the current version of this library
	/// </returns>
	public static ImmutableSemanticVersion GetUtilsVersion() => new ImmutableSemanticVersion("0.2.0");

	/// <summary>
	/// Gets the display length that a <see cref="string"/> would have if used in <see cref="PrintColoredText(object)"/>
	/// </summary>
	/// <param name="str">The <see cref="string"/> to check the display length</param>
	/// <returns>The display length of <paramref name="str"/></returns>
	public static int GetDisplayLength(string str) {
		ArgumentNullException.ThrowIfNull(str, nameof(str));

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
	/// Parses a <see cref="string"/> <see cref="Array"/> of arguments into a <see cref="Dictionary{TKey, TValue}"/> containing those arguments
	/// </summary>
	/// <param name="args">The argumets to parse</param>
	/// <returns>A <see cref="Dictionary{TKey, TValue}"/> with the argumets parsed</returns>
	public static Dictionary<string, string> ArgsParser(string[] args) {
		ArgumentNullException.ThrowIfNull(args, nameof(args));

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
	/// Takes an <see cref="IEnumerable{T}"/> and shuffles it
	/// </summary>
	/// <typeparam name="T">The type of the elements on the <see cref="IEnumerable{T}"/></typeparam>
	/// <param name="list">The <see cref="IEnumerable{T}"/> to shuffle</param>
	/// <param name="random">An instance of <see cref="Random"/> to use</param>
	public static IEnumerable<T> Shuffle<T>(IEnumerable<T> list, Random? random = null) {
		ArgumentNullException.ThrowIfNull(list, nameof(list));
		random ??= new Random();
		return list.OrderBy(t => random.Next());
	}

	/// <summary>
	/// Converts the first 8 <see cref="bool"/>s in a <see cref="bool"/> <see cref="Array"/> to a <see cref="byte"/>
	/// </summary>
	/// <param name="bits">The bits to convert to a <see cref="byte"/></param>
	/// <returns>A <see cref="byte"/> made of the <see cref="bool"/>s</returns>
	public static byte BitsToByte(bool[] bits) {
		ArgumentNullException.ThrowIfNull(bits, nameof(bits));
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
	/// Converts a <see cref="byte"/> in a 8-length <see cref="bool"/> <see cref="Array"/>
	/// </summary>
	/// <param name="b">The <see cref="byte"/> to convert to a <see cref="bool"/> <see cref="Array"/></param>
	/// <returns>A <see cref="bool"/> <see cref="Array"/> made of the <see cref="byte"/></returns>
	public static bool[] ByteToBits(byte b) {
		ArgumentNullException.ThrowIfNull(b, nameof(b));
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
	/// Converts a hex <see cref="string"/> to a <see cref="byte"/> <see cref="Array"/>
	/// </summary>
	/// <param name="hex">The hex <see cref="string"/> to convert</param>
	/// <returns>
	/// A <see cref="byte"/> <see cref="Array"/> representing <paramref name="hex"/>
	/// </returns>
	public static byte[] HexStringToByteArray(string hex) {
		ArgumentNullException.ThrowIfNull(hex, nameof(hex));
		return Enumerable.Range(0, hex.Length)
						 .Where(x => x % 2 == 0)
						 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
						 .ToArray();
	}

	/// <summary>
	/// The dafault initalization vector for AES encryption/decryption
	/// </summary>
	public static readonly ImmutableArray<byte> iv = new byte[16] { 30, 188, 169, 243, 90, 122, 131, 243, 83, 72, 206, 153, 144, 119, 198, 91 }.ToImmutableArray();

	/// <summary>
	/// Encrypts a <see cref="byte"/> <see cref="Array"/> of text with the AES cryptographic algorithm
	/// </summary>
	/// <param name="data">The data to encrypt</param>
	/// <param name="key">The key to use for the encryption</param>
	/// <param name="iv">The iv to use for the decription; if not specified, <see cref="iv"/> is used</param>
	/// <returns>
	/// The data from <paramref name="data"/>, encrypted
	/// </returns>
	public static byte[] AesEncrypt(byte[] data, byte[] key, byte[]? iv = null) {
		iv ??= Utils.iv.ToArray();
		ArgumentNullException.ThrowIfNull(data, nameof(data));
		ArgumentNullException.ThrowIfNull(key, nameof(key));
		if (data.Length == 0)
			throw new ArgumentException($"{nameof(data)} must not have a length of zero", nameof(data));
		if (key.Length != 32)
			throw new ArgumentException($"{nameof(key)} has to have a length of 32", nameof(key));

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
					csEncrypt.Write(data, 0, data.Length);
					csEncrypt.FlushFinalBlock();

					return msEncrypt.ToArray();
				}
			}
		}
	}

	/// <summary>
	/// Decrypts a <see cref="byte"/> <see cref="Array"/> with the AES cryptographic algorithm
	/// </summary>
	/// <param name="data">The data to decrypt</param>
	/// <param name="key">The key to use for the decryption</param>
	/// <param name="iv">The iv to use for the decription; if not specified, <see cref="iv"/> is used</param>
	/// <returns>
	/// The data from <paramref name="data"/>, decrypted
	/// </returns>
	public static byte[] AesDecrypt(byte[] data, byte[] key, byte[]? iv = null) {
		iv ??= Utils.iv.ToArray();
		ArgumentNullException.ThrowIfNull(data, nameof(data));
		ArgumentNullException.ThrowIfNull(key, nameof(key));
		if (data.Length == 0)
			throw new ArgumentException($"{nameof(data)} must not have a length of zero", nameof(data));
		if (key.Length != 32)
			throw new ArgumentException($"{nameof(key)} has to have a length of 32", nameof(key));

		// Create an Aes object
		// with the specified key and IV.
		using (Aes aesAlg = Aes.Create()) {
			aesAlg.Key = key;
			aesAlg.IV = iv;

			// Create a decryptor to perform the stream transform.
			ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for decryption.
			using (MemoryStream msDecrypt = new MemoryStream()) {
				using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write)) {
					csDecrypt.Write(data, 0, data.Length);
					csDecrypt.FlushFinalBlock();

					return msDecrypt.ToArray();
				}
			}
		}
	}

	/// <summary>
	/// Hashes the <paramref name="inputString"/> with <see cref="SHA256"/>
	/// </summary>
	/// <param name="inputString">The <see cref="string"/> to hash with <see cref="SHA256"/></param>
	/// <returns>The <see cref="SHA256"/>'d hashed version of the <paramref name="inputString"/></returns>
	public static string GetHashString(string inputString) {
		ArgumentNullException.ThrowIfNull(inputString, nameof(inputString));
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
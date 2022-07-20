using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using static Utils.Utils;

/*
 * This is excluded from code coverage because it's visual and idk how to test it
 */

namespace Utils;

/// <summary>
/// A simple logging class
/// </summary>
[ExcludeFromCodeCoverage]
public static class Logger {
	static Logger() {
		AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
	}

	private static string log = string.Empty;

	/// <summary>
	/// Whether or not to log to a file
	/// </summary>
	public static bool logToFile { get; set; } = true;

	/// <summary>
	/// Only print to the console messages with a level with a lower or equal value than this (lower => more severe)
	/// </summary>
	public static int visibilityLevel { get; set; } = 3;

	/// <summary>
	/// The directory to output the log files to if <see cref="logToFile"/> is enabled
	/// </summary>
	public static string logDirectory { get; set; } = "logs";

	/// <summary>
	/// Logs a message in the debug level<br/>
	/// Used for very specific debug messages
	/// </summary>
	/// <param name="message">The message to log</param>
	public static void Debug(object message) {
		TranslatableLog(4, "logger.prefix.debug", message);
	}

	/// <summary>
	/// Logs a message in the info level<br/>
	/// Used for informational messages
	/// </summary>
	/// <param name="message">The message to log</param>
	public static void Info(object message) {
		TranslatableLog(3, "logger.prefix.info", message);
	}

	/// <summary>
	/// Logs a message in the warn level<br/>
	/// Used for problems that will lead to broken stuff, but most of the application will remain operational<br/>
	/// or when undesired behaviour from the user is recieved
	/// </summary>
	/// <param name="message">The message to log</param>
	public static void Warn(object message) {
		TranslatableLog(2, "logger.prefix.warn", message);
	}

	/// <summary>
	/// Logs a message in the error level<br/>
	/// Used for problems that will lead to a lot of broken stuff, but some of the application will remain operational
	/// </summary>
	/// <param name="message">The message to log</param>
	public static void Error(object message) {
		TranslatableLog(1, "logger.prefix.error", message);
	}

	/// <summary>
	/// Logs a message in the fatal level<br/>
	/// Used for application-breaking problems
	/// </summary>
	/// <param name="message">The message to log</param>
	public static void Fatal(object message) {
		TranslatableLog(0, "logger.prefix.fatal", message);
	}

	/// <summary>
	/// Logs a message at the specified level but with a custom translatable prefix
	/// </summary>
	/// <param name="level">The level of the <paramref name="message"/></param>
	/// <param name="prefixKey">The translation key of the custom prefix for this <paramref name="message"/></param>
	/// <param name="message">The message to log</param>
	public static void TranslatableLog(int level, string prefixKey, object message) {
		Log(level, new TranslatableText(prefixKey, Assembly.GetCallingAssembly()), message);
	}

	/// <summary>
	/// Logs a message at the specified level but with a custom prefix
	/// </summary>
	/// <param name="level">The level of the <paramref name="message"/></param>
	/// <param name="prefix">The custom prefix for this <paramref name="message"/></param>
	/// <param name="message">The message to log</param>
	public static void Log(int level, string prefix, object message) {
		if (logToFile)
			log += $"[{DateTime.Now:HH:mm:ss}] " + prefix + message.ToString() + "\n";

		if (level > visibilityLevel)
			return;

		string splitMessage = string.Join('\n', SplitBySpaceAndLength(prefix, message.ToString()));
		PrintColoredText(prefix + splitMessage.Replace("\n", "\n" + new string(' ', GetDisplayLength(prefix))));
	}

	private static IEnumerable<string> SplitBySpaceAndLength(string prefix, string message) {
		return Regex.Split(message, @"(.{1," + (Console.WindowWidth - GetDisplayLength(prefix) - 1).ToString() + @"})(?:\s|$)").Where(x => x != string.Empty);
	}

	/// <summary>
	/// Saves all the logged messages to a file and clears them
	/// </summary>
	public static void SaveLog() {
		if (log != string.Empty) {
			Directory.CreateDirectory(logDirectory);
			File.WriteAllText($"{logDirectory}/{DateTime.Now:yyyy-MM-dd HH.mm.ss}.txt", log);
			log = string.Empty;
		}
	}

	private static void OnProcessExit(object sender, EventArgs e) {
		SaveLog();
	}
}
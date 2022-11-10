using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace Utils;

/// <summary>
/// A class that takes a key and returns a <see cref="string"/> based on the data
/// </summary>
public class TranslatableText : IEquatable<TranslatableText> {
	/// <summary>
	/// Creates a new <see cref="TranslatableText"/> based on a translation key
	/// </summary>
	/// <param name="key">The translation key</param>
	/// <param name="args">The args that'll be passed into <see cref="string.Format(string, object[])"/></param>
	public TranslatableText(string key, params object[] args) : this(key, Assembly.GetCallingAssembly(), args) {}

	/// <summary>
	/// Creates a new <see cref="TranslatableText"/> based on a translation key
	/// </summary>
	/// <param name="key">The translation key</param>
	/// <param name="assembly">The <see cref="Assembly"/> to proxy</param>
	/// <param name="args">The args that'll be passed into <see cref="string.Format(string, object[])"/></param>
	public TranslatableText(string key, Assembly assembly, params object[] args) {
		ArgumentNullException.ThrowIfNull(key, nameof(key));
		ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));
		this.key = key;
		this.args = args;
		this.assembly = assembly;

		//immediatly throw an error if it'd throw one upon use
		_ = ToString();
	}

	static TranslatableText() {
		/*
		{
			"logger": {
				"prefix": {
					"debug": "§g§f[§bDEBUG§f]§r ",
					"info": "§g§f[§aINFO§f]§r ",
					"warn": "§g§f[§eWARN§f]§r ",
					"error": "§g§f[§cERROR§f]§r ",
					"fatal": "§g§f[§f§tFATAL§f§y]§r "
				}
			}
		}
		*/
		TranslatableTextSettings settings = new TranslatableTextSettings {
			error = false,
			data = JsonSerializer.Deserialize<JsonElement>("{\"logger\":{\"prefix\":{\"debug\":\"§g§f[§bDEBUG§f]§r \",\"info\": \"§g§f[§aINFO§f]§r \",\"warn\": \"§g§f[§eWARN§f]§r \",\"error\": \"§g§f[§cERROR§f]§r \",\"fatal\": \"§g§f[§f§tFATAL§f§y]§r \"}}}")
		};
		SetSettings(settings);
	}

	/// <summary>
	/// An <see cref="Exception"/> that is thrown when a <see cref="TranslatableText"/> is initialized with a non-existent key and the <see cref="TranslatableTextSettings.error"/> option is set to <c>true</c>
	/// </summary>
	public class TranslatableKeyNotFoundException : Exception {
		internal TranslatableKeyNotFoundException(string message) : base(message) {}
	}

	/// <summary>
	/// The setting used for <see cref="TranslatableText"/> in an <see cref="Assembly"/>
	/// </summary>
	public sealed record TranslatableTextSettings {
		/// <summary>
		/// The data used for obtaining the value of the keys
		/// </summary>
		public JsonElement data { get; set; } = JsonSerializer.Deserialize<JsonElement>("{}");

		/// <summary>
		/// Whether or not to error when not finding a key in the data
		/// </summary>
		public bool error { get; set; } = true;

		/// <summary>
		/// The instance of <see cref="Random"/> used for getting a value within arrays
		/// </summary>
		public Random random { get; set; } = new Random();

		/// <summary>
		/// The character that marks the change of subKey
		/// </summary>
		public char subKeySplitCharacter { get; set; } = '.';
	}

	private readonly string key;
	private readonly object[] args;
	private Assembly assembly;

	private static readonly Dictionary<Assembly, TranslatableTextSettings> settingsByAssembly = new Dictionary<Assembly, TranslatableTextSettings>();

	/// <summary>
	/// Tries to set the <see cref="TranslatableTextSettings"/> for the <see cref="Assembly"/> that calls this method
	/// </summary>
	/// <param name="translatableTextSettings">The <see cref="TranslatableTextSettings"/> to set for the calling <see cref="Assembly"/></param>
	/// <param name="assembly">The assembly to set the settings for or null to set it for the calling assembly</param>
	/// <returns>
	/// true if it succeded setting the <see cref="TranslatableTextSettings"/>, false if it failed
	/// </returns>
	public static void SetSettings(TranslatableTextSettings translatableTextSettings, Assembly? assembly = null) {
		ArgumentNullException.ThrowIfNull(translatableTextSettings, nameof(translatableTextSettings));
		assembly ??= Assembly.GetCallingAssembly();
		if (settingsByAssembly.ContainsKey(assembly)) {
			settingsByAssembly[assembly] = translatableTextSettings;
		} else {
			settingsByAssembly.Add(assembly, translatableTextSettings);
		}
	}

	/// <summary>
	/// Get the <see cref="TranslatableTextSettings"/> for the calling <see cref="Assembly"/>
	/// </summary>
	/// <returns>
	/// The <see cref="TranslatableTextSettings"/> for the calling <see cref="Assembly"/>
	/// </returns>
	public static TranslatableTextSettings GetSettings() => settingsByAssembly[Assembly.GetCallingAssembly()];
	
	/// <summary>
	/// An operator that implicitly converts a <see cref="TranslatableText"/> into a <see cref="string"/>
	/// </summary>
	/// <param name="t">The <see cref="TranslatableText"/> to convert into a <see cref="string"/></param>
	public static implicit operator string(TranslatableText t) => t.ToString();

	/// <summary>
	/// Creates a deep copy of this <see cref="TranslatableText"/>
	/// </summary>
	/// <returns>A deep copy of this <see cref="TranslatableText"/></returns>
	public TranslatableText Clone() => new TranslatableText(key, assembly, args);

	/// <inheritdoc/>
	override public bool Equals(object? obj) => (obj is TranslatableText || obj is string) && obj.ToString() == ToString();

	/// <inheritdoc/>
	public bool Equals(TranslatableText? other) => other is not null && other.ToString() == ToString();

	/// <inheritdoc/>
	public static bool operator ==(TranslatableText left, TranslatableText right) => left is null ? right is null : left.Equals(right);

	/// <inheritdoc/>
	public static bool operator !=(TranslatableText left, TranslatableText right) => !(left == right);

	/// <inheritdoc/>
	override public int GetHashCode() => ToString().GetHashCode();

	/// <inheritdoc/>
	override public string ToString() {
		TranslatableTextSettings settings = settingsByAssembly[assembly];
		//we get the lang data and set it as the first element
		JsonElement currentJsonElement = settings.data;
		
		//we split the key into it's subKeys
		string[] subKeys = key.Split(settings.subKeySplitCharacter);

		for (int i = 0; i < subKeys.Length; i += 1) {
			//we get the next JsonElement and its JsonValueKind
			currentJsonElement.TryGetProperty(subKeys[i], out JsonElement nextJsonElement);
			JsonValueKind type = nextJsonElement.ValueKind;

			//if the next element has an Undefined JsonValueKind that means that the next element is not contained in the current element
			//we then either throw an error or set the key to the value depending on the setting
			if (type == JsonValueKind.Undefined) {
				if (settings.error) {
					throw new TranslatableKeyNotFoundException($"{currentJsonElement} doesn't contain {subKeys[i]}");
				} else {
					return key;
				}
			}

			if (i == subKeys.Length - 1) {//if the current subKey is the last subkey
				if (type == JsonValueKind.String) {//if it has a JsonValueKind of String we set value to it
					return string.Format(nextJsonElement.ToString(), args);
				} else if (type == JsonValueKind.Array) {//if instead of a string, it's an array, we set value to a random string in that array using the static System.Random instance
					return string.Format(nextJsonElement[settings.random.Next(nextJsonElement.GetArrayLength())].ToString(), args);
				} else {//if it's not a string or an array, something has gone wrong so we throw an error or set the value to the key depending on the setting
					if (settings.error) {
						throw new TranslatableKeyNotFoundException($"{nextJsonElement}'s JsonValueKind is {nextJsonElement.ValueKind}, not JsonValueKind.String or JsonValueKind.Array");
					} else {
						return key;
					}
				}
			} else {//if the current subKey is not last subkey
				if (type == JsonValueKind.Object) {//it has to have a JsonValueKind of Object to be able continue descending the tree
					currentJsonElement = nextJsonElement;
				} else {//if it's not an object, something has gone wrong so we throw an error or set the value to the key depending on the setting
					if (settings.error) {
						throw new TranslatableKeyNotFoundException($"{nextJsonElement}'s JsonValueKind is {nextJsonElement.ValueKind}, not JsonValueKind.Object");
					} else {
						return key;
					}
				}
			}
		}

		/*
		 * this exception should never be reached and is only here so that it stops saying that not all code routes return a value
		 * this would only be reached if string.Split() returned an empty array, or an array with negative length; which can't happen
		 */
		throw new Exception("Something has gone terribly wrong");
	}

	/// <summary>
	/// Calls <see cref="ToString()"/> as a specific <see cref="Assembly"/>
	/// </summary>
	/// <param name="assembly">The <see cref="Assembly"/> to call <see cref="ToString()"/> as</param>
	/// <returns>
	/// The result of <see cref="ToString()"/> if it was called by a specific <see cref="Assembly"/>
	/// </returns>
	public string ToStringAsAssembly(Assembly assembly) {
		Assembly currentAssembly = this.assembly;
		this.assembly = assembly;
		string str = ToString();
		this.assembly = currentAssembly;
		return str;
	}
}
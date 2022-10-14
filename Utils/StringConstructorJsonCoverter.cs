using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utils;

/// <summary>
/// Supports converting any <see cref="Type"/> with a <see cref="string"/> constructor by using a factory pattern
/// </summary>
public class StringConstructorJsonCoverterFactory : JsonConverterFactory {
	/// <inheritdoc/>
	override public bool CanConvert(Type typeToConvert) {
		return typeToConvert.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(string) }, null) is not null;
	}

	/// <inheritdoc/>
	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
		Type genericTestType = typeof(StringConstructorJsonCoverter<>).MakeGenericType(typeToConvert);
		return (JsonConverter)Activator.CreateInstance(genericTestType)!;
	}
}

/// <summary>
/// A <see cref="JsonConverter"/> that coverts <see cref="object"/>s that have a constructor that takes only a <see cref="string"/>
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of the <see cref="object"/></typeparam>
public class StringConstructorJsonCoverter<T> : JsonConverter<T> {
	/// <inheritdoc/>
	public override bool CanConvert(Type typeToConvert) {
		return typeToConvert.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(string) }, null) is not null;
	}

	/// <inheritdoc/>
	override public T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		return (T)(typeToConvert.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(string) }, null)?.Invoke(new object?[] { reader.GetString() }) ?? throw new ArgumentException($"{typeToConvert} did not have a valid constructor", nameof(typeToConvert)));
	}

	/// <inheritdoc/>
	override public T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		return (T)(typeToConvert.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(string) }, null)?.Invoke(new object?[] { reader.GetString() }) ?? throw new ArgumentException($"{typeToConvert} did not have a valid constructor", nameof(typeToConvert)));
	}

	/// <inheritdoc/>
	override public void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
		ArgumentNullException.ThrowIfNull(value, nameof(value));
		writer.WriteStringValue(value.ToString());
	}

	/// <inheritdoc/>
	override public void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
		ArgumentNullException.ThrowIfNull(value, nameof(value));
		writer.WriteStringValue(value.ToString());
	}
}
using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utils;

/// <summary>
/// Supports converting any type with a string constructor by using a factory pattern
/// </summary>
public class StringConstructorJsonCoverterFactory : JsonConverterFactory {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="typeToConvert"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public bool CanConvert(Type typeToConvert) {
		return typeToConvert.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(string) }, null) is not null;
	}

	/// <summary>
	/// Creates a <see cref="StringConstructorJsonCoverter{T}"/> for the specified type
	/// </summary>
	/// <param name="typeToConvert"><inheritdoc/></param>
	/// <param name="options"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
		Type genericTestType = typeof(StringConstructorJsonCoverter<>).MakeGenericType(typeToConvert);
		return (JsonConverter)Activator.CreateInstance(genericTestType)!;
	}
}

/// <summary>
/// A <see cref="JsonConverter"/> that coverts <see cref="object"/>s that have a constructor that takes only a string
/// </summary>
/// <typeparam name="T"></typeparam>
public class StringConstructorJsonCoverter<T> : JsonConverter<T> {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="typeToConvert"><inheritdoc/></param>
	/// <returns>
	/// <inheritdoc/>
	/// </returns>
	public override bool CanConvert(Type typeToConvert) {
		return typeToConvert.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(string) }, null) is not null;
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="reader"><inheritdoc/></param>
	/// <param name="typeToConvert"><inheritdoc/></param>
	/// <param name="options"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		return (T)(typeToConvert.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(string) }, null)?.Invoke(new object?[] { reader.GetString() }) ?? throw new ArgumentException($"{typeToConvert} did not have a valid constructor", nameof(typeToConvert)));
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="reader"><inheritdoc/></param>
	/// <param name="typeToConvert"><inheritdoc/></param>
	/// <param name="options"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		return (T)(typeToConvert.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(string) }, null)?.Invoke(new object?[] { reader.GetString() }) ?? throw new ArgumentException($"{typeToConvert} did not have a valid constructor", nameof(typeToConvert)));
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="writer"><inheritdoc/></param>
	/// <param name="value"><inheritdoc/></param>
	/// <param name="options"><inheritdoc/></param>
	override public void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
		ArgumentNullException.ThrowIfNull(value, nameof(value));
		writer.WriteStringValue(value.ToString());
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="writer"><inheritdoc/></param>
	/// <param name="value"><inheritdoc/></param>
	/// <param name="options"><inheritdoc/></param>
	override public void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options) {
		ArgumentNullException.ThrowIfNull(value, nameof(value));
		writer.WriteStringValue(value.ToString());
	}
}
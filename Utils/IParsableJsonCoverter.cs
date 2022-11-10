using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utils;

/// <summary>
/// Supports converting any <see cref="IParsable{TSelf}"/> by using a factory pattern
/// </summary>
public class IParsableJsonCoverterFactory : JsonConverterFactory {
	/// <inheritdoc/>
	override public bool CanConvert(Type typeToConvert) {
		return typeToConvert.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(string) }, null) is not null;
	}

	/// <inheritdoc/>
	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
		Type genericTestType = typeof(IParsableJsonCoverter<>).MakeGenericType(typeToConvert);
		return (JsonConverter)Activator.CreateInstance(genericTestType)!;
	}
}

/// <summary>
/// A <see cref="JsonConverter"/> that coverts an <see cref="IParsable{TSelf}"/>
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of the <see cref="IParsable{TSelf}"/></typeparam>
public class IParsableJsonCoverter<T> : JsonConverter<T> where T : IParsable<T> {
	/// <inheritdoc/>
	public override bool CanConvert(Type typeToConvert) {
		return typeToConvert.IsAssignableTo(typeof(IParsable<>));
	}

	/// <inheritdoc/>
	override public T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		return T.Parse(reader.GetString(), null);
	}

	/// <inheritdoc/>
	override public T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		return T.Parse(reader.GetString(), null);
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
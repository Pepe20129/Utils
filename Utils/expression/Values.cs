using static Utils.expression.Expression;

namespace Utils.expression;

/// <summary>
/// A wrapper around a value in an <see cref="Expression"/>
/// </summary>
public abstract class Value : Expression.Segment {
	/// <summary>
	/// Creates a new <see cref="Value"/> based on <paramref name="raw"/>
	/// </summary>
	/// <param name="raw">The string representation of this <see cref="Value"/></param>
	/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
	public Value(string raw, Expression? expression) : base(raw, expression) {}

	/// <summary>
	/// The value of this <see cref="Value"/>
	/// </summary>
	public object? value { get; protected set; }

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string? ToString() => value?.ToString();
}

/// <summary>
/// A <see cref="Value"/> wrapper around a <see cref="Expression"/>
/// </summary>
public class SubExpression : Value {
	/// <summary>
	/// Creates a new <see cref="SubExpression"/> based on <paramref name="raw"/>
	/// </summary>
	/// <param name="raw">The string representation of this <see cref="SubExpression"/></param>
	/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
	public SubExpression(string raw, Expression expression) : base(raw, expression) {
		this.expression = new Expression(raw[1..^1]) {
			operatorPrecedences = expression.operatorPrecedences,
			regexStrings = expression.regexStrings
		};
		value = this.expression.Execute().value;
	}

	private readonly Expression expression;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => $"({expression})";
}

/// <summary>
/// A <see cref="Value"/> wrapper around a <c>null</c>
/// </summary>
public class NullValue : Value {
	/// <summary>
	/// Creates a new <see cref="NullValue"/>
	/// </summary>
	/// <param name="raw">Ignored</param>
	/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
	public NullValue(string raw, Expression expression) : base(raw, expression) {
		value = null;
	}

	/// <summary>
	/// Creates a new <see cref="NullValue"/>
	/// </summary>
	public NullValue() : base("null", null) {
		value = null;
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "null";
}

/// <summary>
/// A <see cref="Value"/> wrapper around a <see cref="bool"/>
/// </summary>
public class BooleanValue : Value {
	/// <summary>
	/// Creates a new <see cref="BooleanValue"/> based on the value of <paramref name="raw"/>
	/// </summary>
	/// <param name="raw">The string repersentation of the <see cref="bool"/> value of this</param>
	/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
	public BooleanValue(string raw, Expression expression) : base(raw, expression) {
		value = bool.Parse(raw);
	}

	/// <summary>
	/// Creates a new <see cref="BooleanValue"/> with a specific value
	/// </summary>
	/// <param name="value">The <see cref="bool"/> value</param>
	public BooleanValue(bool value) : base(value.ToString(), null) {
		this.value = value;
	}
}

/// <summary>
/// A <see cref="Value"/> wrapper around a <see cref="int"/>
/// </summary>
public class IntegerValue : Value {
	/// <summary>
	/// Creates a new <see cref="IntegerValue"/> based on the value of <paramref name="raw"/>
	/// </summary>
	/// <param name="raw">The string repersentation of the <see cref="int"/> value of this</param>
	/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
	public IntegerValue(string raw, Expression expression) : base(raw, expression) {
		value = int.Parse(raw);
	}

	/// <summary>
	/// Creates a new <see cref="IntegerValue"/> with a specific value
	/// </summary>
	/// <param name="value">The <see cref="int"/> value</param>
	public IntegerValue(int value) : base(value.ToString(), null) {
		this.value = value;
	}
}

/// <summary>
/// A <see cref="Value"/> wrapper around a <see cref="long"/>
/// </summary>
public class LongValue : Value {
	/// <summary>
	/// Creates a new <see cref="IntegerValue"/> based on the value of <paramref name="raw"/>
	/// </summary>
	/// <param name="raw">The string repersentation of the <see cref="long"/> value of this</param>
	/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
	public LongValue(string raw, Expression expression) : base(raw, expression) {
		value = long.Parse(raw[..^1]);
	}

	/// <summary>
	/// Creates a new <see cref="IntegerValue"/> with a specific value
	/// </summary>
	/// <param name="value">The <see cref="long"/> value</param>
	public LongValue(long value) : base(value.ToString(), null) {
		this.value = value;
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => $"{value}L";
}

/// <summary>
/// A <see cref="Value"/> wrapper around a <see cref="float"/>
/// </summary>
public class FloatValue : Value {
	/// <summary>
	/// Creates a new <see cref="FloatValue"/> based on the value of <paramref name="raw"/>
	/// </summary>
	/// <param name="raw">The string repersentation of the <see cref="float"/> value of this</param>
	/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
	public FloatValue(string raw, Expression expression) : base(raw, expression) {
		value = float.Parse(raw[..^1]);
	}

	/// <summary>
	/// Creates a new <see cref="FloatValue"/> with a specific value
	/// </summary>
	/// <param name="value">The <see cref="float"/> value</param>
	public FloatValue(float value) : base(value.ToString(), null) {
		this.value = value;
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => $"{value}f";
}

/// <summary>
/// A <see cref="Value"/> wrapper around a <see cref="string"/>
/// </summary>
public class StringValue : Value {
	/// <summary>
	/// Creates a new <see cref="StringValue"/> based on the value of <paramref name="raw"/>
	/// </summary>
	/// <param name="raw">The string repersentation of the <see cref="string"/> value of this (sorrounded by <c>"</c>)</param>
	/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
	public StringValue(string raw, Expression expression) : base(raw, expression) {
		value = raw[1..^1];
	}

	/// <summary>
	/// Creates a new <see cref="StringValue"/> with a specific value
	/// </summary>
	/// <param name="value">The <see cref="string"/> value</param>
	public StringValue(string value) : base(value, null) {
		this.value = value;
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => $"\"{(string)value!}\"";
}
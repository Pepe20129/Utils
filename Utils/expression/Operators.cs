using System;
using static Utils.expression.Expression;

namespace Utils.expression;

/// <summary>
/// A <see cref="Expression.Segment"/> that operates on <see cref="Value"/>s
/// </summary>
public abstract class Operator : Expression.Segment {
	/// <summary>
	/// Creates a new <see cref="Operator"/> based on <paramref name="raw"/>
	/// </summary>
	/// <param name="raw">The string representation of this <see cref="Operator"/></param>
	/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
	public Operator(string raw, Expression expression) : base(raw, expression) {}
}

/// <summary>
/// An <see cref="Operator"/> that precedes a value and modifies it
/// </summary>
public abstract class UnaryOperator : Operator {
	/// <summary>
	/// Creates a new <see cref="UnaryOperator"/> based on <paramref name="raw"/>
	/// </summary>
	/// <param name="raw">The string representation of this <see cref="UnaryOperator"/></param>
	/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
	public UnaryOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// Whether or not this <see cref="UnaryOperator"/> can operate on <paramref name="value"/>
	/// </summary>
	/// <param name="value">The value to check operatibility for</param>
	/// <returns>Whether or not this <see cref="UnaryOperator"/> can operate on <paramref name="value"/></returns>
	public abstract bool CanOperate(Value value);

	/// <summary>
	/// Operates this <see cref="UnaryOperator"/> on <paramref name="value"/>
	/// </summary>
	/// <param name="value">The <see cref="UnaryOperator"/>'s argument</param>
	/// <returns>The result of operating this <see cref="UnaryOperator"/> on <paramref name="value"/></returns>
	public abstract Value Operate(Value value);
}

/// <summary>
/// Inverts a <see cref="BooleanValue"/>
/// </summary>
public class NegationOperator : UnaryOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public NegationOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="value"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override bool CanOperate(Value value) => value.value is bool;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="value"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value value) => new BooleanValue(!(bool)value.value);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "!";
}

/// <summary>
/// Inverts every bit of a <see cref="Value"/>
/// </summary>
public class BitwiseComplementOperator : UnaryOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public BitwiseComplementOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="value"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override bool CanOperate(Value value) => value.value is int || value.value is long;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="value"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value value) => new IntegerValue(~(int)value.value);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "~";
}

/// <summary>
/// An <see cref="Operator"/> that takes the preceding and the succeding <see cref="Value"/> and returns a <see cref="Value"/>
/// </summary>
public abstract class DualOperator : Operator {
	/// <summary>
	/// Creates a new <see cref="DualOperator"/> based on <paramref name="raw"/>
	/// </summary>
	/// <param name="raw">The string representation of this <see cref="DualOperator"/></param>
	/// <param name="expression"><inheritdoc/></param>
	public DualOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// Whether or not this <see cref="DualOperator"/> can operate on <paramref name="left"/> &amp; <paramref name="right"/>
	/// </summary>
	/// <param name="left">The first value to check operatibility for</param>
	/// <param name="right">The second value to check operatibility for</param>
	/// <returns>Whether or not this <see cref="DualOperator"/> can operate on <paramref name="left"/> &amp; <paramref name="right"/></returns>
	public abstract bool CanOperate(Value left, Value right);

	/// <summary>
	/// Operates this <see cref="DualOperator"/> on <paramref name="left"/> &amp; <paramref name="right"/>
	/// </summary>
	/// <param name="left">The <see cref="DualOperator"/>'s first argument</param>
	/// <param name="right">The <see cref="DualOperator"/>'s second argument</param>
	/// <returns>The result of operating this <see cref="DualOperator"/> on <paramref name="left"/> &amp; <paramref name="right"/></returns>
	public abstract Value Operate(Value left, Value right);
}

/// <summary>
/// A <see cref="DualOperator"/> that operates on <see cref="BooleanValue"/>s
/// </summary>
public abstract class BooleanOperator : DualOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public BooleanOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override bool CanOperate(Value left, Value right) => left.value is bool && right.value is bool;
}

/// <summary>
/// Conditional ANDs two <see cref="BooleanValue"/>s
/// </summary>
public class AndOperator : BooleanOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public AndOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public Value Operate(Value left, Value right) => new BooleanValue((bool)left.value && (bool)right.value);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "&&";
}

/// <summary>
/// Logical ANDs two <see cref="BooleanValue"/>s
/// </summary>
public class LogicalAndOperator : BooleanOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public LogicalAndOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public Value Operate(Value left, Value right) => new BooleanValue((bool)left.value & (bool)right.value);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "&";
}

/// <summary>
/// Conditional ORs two <see cref="BooleanValue"/>s
/// </summary>
public class OrOperator : BooleanOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public OrOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public Value Operate(Value left, Value right) => new BooleanValue((bool)left.value || (bool)right.value);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "||";
}

/// <summary>
/// Logical ORs two <see cref="BooleanValue"/>s
/// </summary>
public class LogicalOrOperator : BooleanOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public LogicalOrOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public Value Operate(Value left, Value right) => new BooleanValue((bool)left.value | (bool)right.value);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "|";
}

/// <summary>
/// Logical XORs two <see cref="BooleanValue"/>s
/// </summary>
public class LogicalXOrOperator : BooleanOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public LogicalXOrOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public Value Operate(Value left, Value right) => new BooleanValue((bool)left.value ^ (bool)right.value);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "^";
}

/// <summary>
/// A <see cref="DualOperator"/> that operates on <see cref="IntegerValue"/>s, <see cref="LongValue"/>s &amp; <see cref="FloatValue"/>s
/// </summary>
public abstract class NumberOperator : DualOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public NumberOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public bool CanOperate(Value left, Value right) => (left.value is int || left.value is float || left.value is long) &&
																(right.value is int || right.value is float || right.value is long);
}

/// <summary>
/// Adds two <see cref="IntegerValue"/>s, <see cref="LongValue"/>s or <see cref="FloatValue"/>s
/// </summary>
public class AdditionOperator : NumberOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public AdditionOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override bool CanOperate(Value left, Value right) => (left.value is string && right.value is string) || base.CanOperate(left, right);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) {
		if ((left.value is long && right.value is long) || (left.value is long && right.value is int) || (left.value is int && right.value is long)) {
			return new LongValue(Convert.ToInt64(left.value) + Convert.ToInt64(right.value));
		}
		if (left.value is int leftInt && right.value is int rightInt) {
			return new IntegerValue(leftInt + rightInt);
		}
		if (left.value is string leftString && right.value is string rightString) {
			return new StringValue(leftString + rightString);
		}
		return new FloatValue(Convert.ToSingle(left.value) + Convert.ToSingle(right.value));
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "+";
}

/// <summary>
/// Substracts two <see cref="IntegerValue"/>s, <see cref="LongValue"/>s or <see cref="FloatValue"/>s
/// </summary>
public class SubstractionOperator : NumberOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public SubstractionOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) {
		if ((left.value is long && right.value is long) || (left.value is long && right.value is int) || (left.value is int && right.value is long)) {
			return new LongValue(Convert.ToInt64(left.value) - Convert.ToInt64(right.value));
		}
		if (left.value is int leftInt && right.value is int rightInt) {
			return new IntegerValue(leftInt - rightInt);
		}
		return new FloatValue(Convert.ToSingle(left.value) - Convert.ToSingle(right.value));
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "-";
}

/// <summary>
/// Multiplies two <see cref="IntegerValue"/>s, <see cref="LongValue"/>s or <see cref="FloatValue"/>s
/// </summary>
public class MultiplicationOperator : NumberOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public MultiplicationOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) {
		if ((left.value is long && right.value is long) || (left.value is long && right.value is int) || (left.value is int && right.value is long)) {
			return new LongValue(Convert.ToInt64(left.value) * Convert.ToInt64(right.value));
		}
		if (left.value is int leftInt && right.value is int rightInt) {
			return new IntegerValue(leftInt * rightInt);
		}
		return new FloatValue(Convert.ToSingle(left.value) * Convert.ToSingle(right.value));
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "*";
}

/// <summary>
/// Divides two <see cref="IntegerValue"/>s, <see cref="LongValue"/>s or <see cref="FloatValue"/>s
/// </summary>
public class DivisionOperator : NumberOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public DivisionOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) {
		if ((left.value is long && right.value is long) || (left.value is long && right.value is int) || (left.value is int && right.value is long)) {
			return new LongValue(Convert.ToInt64(left.value) / Convert.ToInt64(right.value));
		}
		if (left.value is int leftInt && right.value is int rightInt) {
			return new IntegerValue(leftInt / rightInt);
		}
		return new FloatValue(Convert.ToSingle(left.value) / Convert.ToSingle(right.value));
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "/";
}

/// <summary>
/// Divides two <see cref="IntegerValue"/>s, <see cref="LongValue"/>s or <see cref="FloatValue"/>s and takes the remainder
/// </summary>
public class RemainderOperator : DualOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public RemainderOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) {
		if (left.value is long || right.value is long) {
			return new LongValue(Convert.ToInt64(left.value) % Convert.ToInt64(right.value));
		}
		return new IntegerValue((int)left.value % (int)right.value);
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public bool CanOperate(Value left, Value right) => (left.value is int || left.value is int) && (right.value is int || right.value is int);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "%";
}

/// <summary>
/// Checks if two <see cref="Value"/>s are equal
/// </summary>
public class EqualityOperator : DualOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public EqualityOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override bool CanOperate(Value left, Value right) => true;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) => new BooleanValue(left.value.Equals(right.value));

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "==";
}

/// <summary>
/// Checks if two <see cref="Value"/>s are not equal
/// </summary>
public class InequalityOperator : DualOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public InequalityOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override bool CanOperate(Value left, Value right) => true;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) => new BooleanValue(!left.value.Equals(right.value));

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "!=";
}

/// <summary>
/// Checks if a <see cref="Value"/> is greater than or equal to another <see cref="Value"/>
/// </summary>
public class GreaterThanOrEqualToOperator : NumberOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public GreaterThanOrEqualToOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) {
		if (left.value is int leftInt && right.value is int rightInt) {
			return new BooleanValue(leftInt >= rightInt);
		}
		return new BooleanValue(Convert.ToSingle(left.value) >= Convert.ToSingle(right.value));
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => ">=";
}

/// <summary>
/// Checks if a <see cref="Value"/> is lesser than or equal to another <see cref="Value"/>
/// </summary>
public class LesserThanOrEqualToOperator : NumberOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public LesserThanOrEqualToOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) {
		if (left.value is int leftInt && right.value is int rightInt) {
			return new BooleanValue(leftInt <= rightInt);
		}
		return new BooleanValue(Convert.ToSingle(left.value) <= Convert.ToSingle(right.value));
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "<=";
}

/// <summary>
/// Checks if a <see cref="Value"/> is greater than another <see cref="Value"/>
/// </summary>
public class GreaterThanOperator : NumberOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public GreaterThanOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) {
		if (left.value is int leftInt && right.value is int rightInt) {
			return new BooleanValue(leftInt > rightInt);
		}
		return new BooleanValue(Convert.ToSingle(left.value) > Convert.ToSingle(right.value));
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => ">";
}

/// <summary>
/// Checks if a <see cref="Value"/> is lesser than another <see cref="Value"/>
/// </summary>
public class LesserThanOperator : NumberOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public LesserThanOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) {
		if (left.value is int leftInt && right.value is int rightInt) {
			return new BooleanValue(leftInt < rightInt);
		}
		return new BooleanValue(Convert.ToSingle(left.value) < Convert.ToSingle(right.value));
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "<";
}

/// <summary>
/// Bitwise shifts the first <see cref="Value"/> to the left the amount of times dictated by the second <see cref="Value"/>
/// </summary>
public class LeftShiftOperator : DualOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public LeftShiftOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) => new IntegerValue((int)left.value << (int)right.value);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public bool CanOperate(Value left, Value right) => left.value is int && right.value is int;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "<<";
}

/// <summary>
/// Bitwise shifts the first <see cref="Value"/> to the right the amount of times dictated by the second <see cref="Value"/>
/// </summary>
public class RightShiftOperator : DualOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public RightShiftOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) => new IntegerValue((int)left.value >> (int)right.value);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public bool CanOperate(Value left, Value right) => left.value is int && right.value is int;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => ">>";
}

/// <summary>
/// Returns the first <see cref="Value"/>, unless it is <code>null</code>, when it returns the second <see cref="Value"/>
/// </summary>
public class NullCoalescingOperator : DualOperator {
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="raw"><inheritdoc/></param>
	/// <param name="expression"><inheritdoc/></param>
	public NullCoalescingOperator(string raw, Expression expression) : base(raw, expression) {}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override bool CanOperate(Value left, Value right) => true;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public override Value Operate(Value left, Value right) => left.value is null ? right : left;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() => "??";
}
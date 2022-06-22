using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Utils.expression;

namespace UtilsTest;

[TestClass]
[ExcludeFromCodeCoverage]
public class ExpressionTest {
	static readonly Dictionary<string, object> expressions = new Dictionary<string, object> {
		{ "true", true },
		{ "!(!(true || true) || false)", true },
		{ "true || true && false", true },
		{ "false && true || true", true },
		{ "!true", false },
		{ "!true || (true && !true)", false },
		{ "2 + 2", 4 },
		{ "2f + 2", 4f },
		{ "2 + 2f", 4f },
		{ "2f + 2f", 4f },
		{ "10 - 3", 7 },
		{ "10f - 3", 7f },
		{ "10 - 3f", 7f },
		{ "10f - 3f", 7f },
		{ "((45 + 2) - 34) - 13", 0 },
		{ "(45 + 2) - 34 - 13", 0 },
		{ "45 + (2 - 34) - 13", 0 },
		{ "45 + 2 - (34 - 13)", 26 },
		{ "45 + 2 - 34 - 13", 0 },
		{ "3 * 2", 6 },
		{ "3f * 2", 6f },
		{ "3 * 2f", 6f },
		{ "3f * 2f", 6f },
		{ "10 / 2", 5 },
		{ "10 / 3", 3 },
		{ "10 / 3f", 10 / 3f },
		{ "10f / 3", 10f / 3 },
		{ "10 % 6", 4 },
		{ "5 == 5", true },
		{ "5 == 5f", false },
		{ "5 == 4", false },
		{ "5 != 5", false },
		{ "5 != 5f", true },
		{ "5 != 4", true },
		{ "5 == (4 + 1)", true },
		{ "(3 + 2) == 5", true },
		{ "((2 * 2) == 4) && ((3 + 2) == 5)", true },
		{ "5 >= 3", true },
		{ "5 <= 3", false },
		{ "5 > 3", true },
		{ "5 < 3", false },
		{ "5 >= 5", true },
		{ "5 <= 5", true },
		{ "5 > 5", false },
		{ "5 < 5", false },
		{ "5 >= 3f", true },
		{ "5 <= 3f", false },
		{ "5 > 3f", true },
		{ "5 < 3f", false },
		{ "5 >= 5f", true },
		{ "5 <= 5f", true },
		{ "5 > 5f", false },
		{ "5 < 5f", false },
		{ "5f >= 3", true },
		{ "5f <= 3", false },
		{ "5f > 3", true },
		{ "5f < 3", false },
		{ "5f >= 5", true },
		{ "5f <= 5", true },
		{ "5f > 5", false },
		{ "5f < 5", false },
		{ "5f >= 3f", true },
		{ "5f <= 3f", false },
		{ "5f > 3f", true },
		{ "5f < 3f", false },
		{ "5f >= 5f", true },
		{ "5f <= 5f", true },
		{ "5f > 5f", false },
		{ "5f < 5f", false },
		{ "4 << 3", 32 },
		{ "2 >> 1", 1 },
		{ "~2000000000", -2000000001 },
		{ "null", null },
		{ "null ?? true", true },
		{ "true ?? null", true },
		{ "!(!(true | true) | false)", true },
		{ "true | true & false", true },
		{ "false & true | true", true },
		{ "!true | (true & !true)", false },
		{ "true ^ false", true },
		{ "false ^ true", true },
		{ "true ^ true", false },
		{ "false ^ false", false },
		{ "1000000000 * 10L", 10000000000 }
	};

	[TestMethod]
	public void Test() {
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		foreach (KeyValuePair<string, object> pair in expressions) {
			Expression expression = new Expression(pair.Key);
			Assert.IsTrue(expression.ToString().ToLower() == pair.Key.ToLower(), $"{{{expression}}} {{{pair.Key}}}");
			Value value = expression.Execute();
			Assert.IsTrue(value is not null);
			if (value.value is null) {
				Assert.IsTrue(pair.Value is null);
			} else {
				Assert.IsTrue(value.value.Equals(pair.Value), $"{{{value.value} ({value.value.GetType()})}} {{{pair.Value} ({pair.Value.GetType()})}}");
				Assert.IsTrue(value.ToString() == pair.Value.ToString() + (pair.Value is float ? "f" : (pair.Value is long ? "L" : string.Empty)), $"{{{value} ({value.GetType()})}} {{{pair.Value} ({pair.Value.GetType()})}}");
			}
		}

		try {
			_ = new Expression("5 % true").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("true % 5").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("true % true").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("5 && true").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("true && 5").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("5 && 5").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("5 + true").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("false * 2").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("false << 2").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("false >> 2").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("2 << false").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("2 >> false").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("false >> false").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("false << false").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}

		try {
			_ = new Expression("!5").Execute();
			Assert.Fail();
		} catch (Expression.OperatorException) {}
	}

	[TestMethod]
	public void Custom() {
		Expression expression = new Expression("$true ?? 3d");
		expression.regexStrings[typeof(IntegerValue)] = "-?[0-9]+(?<cancel>[FfLlDd])?";
		expression.regexStrings.Add(typeof(DoubleValue), @"-?(?:[0-9]*\.)?[0-9]+[Dd]");
		expression.regexStrings.Add(typeof(NullificationOperator), @"\$");

		if (expression.operatorPrecedences.ContainsKey(0))
			expression.operatorPrecedences[0].Add(typeof(NullificationOperator));
		else
			expression.operatorPrecedences.Add(0, new List<Type> { typeof(NullificationOperator) });
		
		Assert.IsTrue(expression.Execute().value.Equals(new DoubleValue(3d).value), $"{expression.Execute().value.GetType()} {new DoubleValue(3d).value.GetType()}");
	}

	public class NullificationOperator : UnaryOperator {
		public NullificationOperator(string raw, Expression expression) : base(raw, expression) { }

		public override bool CanOperate(Value value) => true;

		public override Value Operate(Value value) => new NullValue();
		
		public override string ToString() => "$";
	}

	public class DoubleValue : Value {
		public DoubleValue(string raw, Expression expression) : base(raw, expression) {
			value = double.Parse(raw[..^1]);
		}

		public DoubleValue(double value) : base(value.ToString(), null) {
			this.value = value;
		}

		override public string ToString() => $"{value}d";
	}
}
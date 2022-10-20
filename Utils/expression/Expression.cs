using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Utils.expression;

/// <summary>
/// A <see cref="Expression"/> that can be executed with <see cref="Execute"/>
/// </summary>
public class Expression {
	/// <summary>
	/// Makes a new <see cref="Expression"/>
	/// </summary>
	/// <param name="raw">The string representation of the <see cref="Expression"/></param>
	public Expression(string raw) {
		ArgumentNullException.ThrowIfNull(raw, nameof(raw));
		this.raw = raw;
	}

	/// <summary>
	/// Makes a new <see cref="Expression"/> with specified regex strings and operator precedences
	/// </summary>
	/// <param name="raw">The string representation of the <see cref="Expression"/></param>
	/// <param name="regexStrings">The regex strings</param>
	/// <param name="operatorPrecedences">The operator precedences</param>
	public Expression(string raw, IDictionary<Type, string> regexStrings, IDictionary<int, List<Type>> operatorPrecedences) : this(raw) {
		ArgumentNullException.ThrowIfNull(regexStrings, nameof(regexStrings));
		ArgumentNullException.ThrowIfNull(operatorPrecedences, nameof(operatorPrecedences));
		this.regexStrings = (Dictionary<Type, string>)regexStrings;
		this.operatorPrecedences = (Dictionary<int, List<Type>>)operatorPrecedences;
	}

	/// <summary>
	/// Executes the <see cref="Expression"/>
	/// </summary>
	/// <returns>The result of the <see cref="Expression"/></returns>
	public Value Execute() {
		string segmentRegexString = string.Empty;
		foreach (KeyValuePair<Type, string> pair in regexStrings) {
			segmentRegexString += pair.Value + "|";
		}

		List<Segment?> segments = new List<Segment?>();
		//..^1 is to remove the last |
		foreach (Match match in new Regex(segmentRegexString[..^1]).Matches(raw))
			segments.Add(CreateSegment(match.Value));

		foreach (KeyValuePair<int, List<Type>> currentOperatorTypes in operatorPrecedences.OrderBy(pair => pair.Key)) {
			int i = 0;
			while (i < segments.Count) {
				//if it's not an operator, or the operator is not on the current "level", ignore it
				if (segments[i] is not Operator || !currentOperatorTypes.Value.Contains(segments[i]!.GetType())) {
					i += 1;
					continue;
				}

				if (segments[i] is DualOperator dualOperator) {
					if (!dualOperator.CanOperate((Value)segments[i - 1]!, (Value)segments[i + 1]!))
						throw new OperatorException($"Operator \"{segments[i]!.GetType()}\" can't operate on \"{segments[i - 1]!.GetType()}\" ({((Value)segments[i - 1]!).value}) & \"{segments[i + 1]!.GetType()}\" ({((Value)segments[i + 1]!).value})");
					segments[i] = dualOperator.Operate((Value)segments[i - 1]!, (Value)segments[i + 1]!);
					segments[i - 1] = null;
					segments[i + 1] = null;
				} else if (segments[i] is UnaryOperator unaryOperator) {
					if (!unaryOperator.CanOperate((Value)segments[i + 1]!))
						throw new OperatorException($"Operator \"{segments[i]!.GetType()}\" can't operate on \"{segments[i + 1]!.GetType()}\" ({((Value)segments[i + 1]!).value})");
					segments[i] = unaryOperator.Operate((Value)segments[i + 1]!);
					segments[i + 1] = null;
				}
				segments = segments.Where(segment => segment is not null).ToList();
				i = 1;
			}
		}
		return (Value)segments[0]!;
	}

	/// <summary>
	/// An <see cref="Exception"/> that is thrown when an operator can't opearte the values it's been given
	/// </summary>
	public class OperatorException : Exception {
		/// <summary>
		/// Creates a new instance of <see cref="OperatorException"/> with a specified message
		/// </summary>
		/// <param name="message">The message of this <see cref="OperatorException"/></param>
		public OperatorException(string message) : base(message) {}
	}

	private Segment CreateSegment(string raw) {
		ArgumentNullException.ThrowIfNull(raw, nameof(raw));
		Segment? segment = null;
		foreach (KeyValuePair<Type, string> pair in regexStrings) {
			Match match = new Regex(pair.Value, RegexOptions.IgnoreCase).Match(raw);
			if (segment is null && match.Value == raw && !match.Groups["cancel"].Success) {
				ConstructorInfo? constructorInfo = pair.Key.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, new Type[] { typeof(string), typeof(Expression) }, null);
				if (constructorInfo is null)
					throw new Exception($"No valid constructor found for type {pair.Key}");
				try {
					segment = (Segment)constructorInfo.Invoke(new object[] { raw, this });
				} catch (TargetInvocationException e) {
					throw e.InnerException ?? e;
				}
				break;
			}
		}

		if (segment is null)
			throw new ArgumentException($"\"{raw}\" is not a valid segment");

		return segment;
	}

	private readonly string raw;

	/// <summary>
	/// Gets the default regex strings
	/// </summary>
	/// <returns>The default regex strings</returns>
	public static Dictionary<Type, string> GetDefaultRegexStrings() {
		return new Dictionary<Type, string> {
			{ typeof(SubExpression), @"\((?:(?<cancel>int|float|bool)|(?>\((?<c>)|[^()]+|\)(?<-c>))*(?(c)(?!)))\)" },
			{ typeof(AndOperator), "&&" },
			{ typeof(OrOperator), @"\|\|" },
			{ typeof(BooleanValue), "true|false" },
			{ typeof(FloatValue), @"-?(?:[0-9]*\.)?[0-9]+[Ff]"},
			{ typeof(LongValue), @"-?(?:[0-9]*\.)?[0-9]+[Ll]"},
			{ typeof(IntegerValue), "-?[0-9]+(?<cancel>[FfLl])?" },
			{ typeof(StringValue), @"\""(?:[^\""\\]|\\.)*\""" },
			{ typeof(AdditionOperator), @"\+" },
			{ typeof(SubstractionOperator), "-" },
			{ typeof(MultiplicationOperator), @"\*" },
			{ typeof(DivisionOperator), "/" },
			{ typeof(RemainderOperator), "%" },
			{ typeof(EqualityOperator), "==" },
			{ typeof(InequalityOperator), "!=" },
			{ typeof(NegationOperator), "!(?<cancel>=)?" },
			{ typeof(GreaterThanOrEqualToOperator), ">=" },
			{ typeof(LesserThanOrEqualToOperator), "<=" },
			{ typeof(RightShiftOperator), ">>" },
			{ typeof(LeftShiftOperator), "<<" },
			{ typeof(GreaterThanOperator), ">(?<cancel>=|>)?" },
			{ typeof(LesserThanOperator), "<(?<cancel>=|<)?" },
			{ typeof(BitwiseComplementOperator), "~" },
			{ typeof(NullValue), "null" },
			{ typeof(NullCoalescingOperator), @"\?\?" },
			{ typeof(LogicalAndOperator), "&(?<cancel>&)?" },
			{ typeof(LogicalOrOperator), @"\|(?<cancel>\|)?" },
			{ typeof(LogicalXOrOperator), @"\^" }
		};
	}

	/// <summary>
	/// Gets the default operator precedences
	/// </summary>
	/// <returns>The default operator precedences</returns>
	public static Dictionary<int, List<Type>> GetDefaultOperatorPrecedences() {
		return new Dictionary<int, List<Type>> {
			{ 1, new List<Type> { typeof(NegationOperator), typeof(BitwiseComplementOperator) } },
			{ 4, new List<Type> { typeof(MultiplicationOperator), typeof(DivisionOperator), typeof(RemainderOperator) } },
			{ 5, new List<Type> { typeof(AdditionOperator), typeof(SubstractionOperator) } },
			{ 6, new List<Type> { typeof(LeftShiftOperator), typeof(RightShiftOperator) } },
			{ 7, new List<Type> { typeof(GreaterThanOrEqualToOperator), typeof(LesserThanOrEqualToOperator), typeof(GreaterThanOperator), typeof(LesserThanOperator) } },
			{ 8, new List<Type> { typeof(EqualityOperator), typeof(InequalityOperator) } },
			{ 9, new List<Type> { typeof(LogicalAndOperator) } },
			{ 10, new List<Type> { typeof(LogicalXOrOperator) } },
			{ 11, new List<Type> { typeof(LogicalOrOperator) } },
			{ 12, new List<Type> { typeof(AndOperator) } },
			{ 13, new List<Type> { typeof(OrOperator) } },
			{ 14, new List<Type> { typeof(NullCoalescingOperator) } }
		};
	}

	/// <summary>
	/// A <see cref="Dictionary{TKey, TValue}"/> that relates the <see cref="Type"/> of a <see cref="Segment"/> with a regex string that matches them<br/>
	/// The regex strings can have the "cancel" capture group, which if matched, the whole regex match will not match
	/// </summary>
	public Dictionary<Type, string> regexStrings = GetDefaultRegexStrings();

	/// <summary>
	/// A <see cref="Dictionary{TKey, TValue}"/> that relates an <see cref="int"/> with a <see cref="List{T}"/> of <see cref="Type"/>s of <see cref="Operator"/>s<br/>
	/// Operators associated with a lower number will be evaluated first
	/// </summary>
	public Dictionary<int, List<Type>> operatorPrecedences = GetDefaultOperatorPrecedences();

	/// <inheritdoc/>
	override public string ToString() => raw;
	
	#pragma warning disable IDE0060
	/// <summary>
	/// A segment within an <see cref="Expression"/>, it can be a <see cref="Value"/> or an <see cref="Operator"/>
	/// </summary>
	public abstract class Segment {
		/// <summary>
		/// Creates a new <see cref="Segment"/> based on <paramref name="raw"/>
		/// </summary>
		/// <param name="raw">The string representation of this <see cref="Segment"/></param>
		/// <param name="expression">The <see cref="Expression"/> of which this <see cref="Segment"/> is a part of</param>
		private protected Segment(string raw, Expression? expression) {}
	}
	#pragma warning restore IDE0060
}
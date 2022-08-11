using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils;

/// <summary>
/// A set of <see cref="SemanticVersionRange"/>s
/// </summary>
public class SemanticVersionRangeSet : IEquatable<SemanticVersionRangeSet> {
	/// <summary>
	/// Parses the <see cref="string"/> provided into a <see cref="SemanticVersionRangeSet"/>
	/// </summary>
	/// <param name="raw">The <see cref="string"/> to parse into a <see cref="SemanticVersionRangeSet"/></param>
	public SemanticVersionRangeSet(string raw) {
		semanticVersionRanges = new List<SemanticVersionRange>();
		string[] ranges = raw.Split("||");
		foreach (string range in ranges) {
			semanticVersionRanges.Add(new SemanticVersionRange(range.Trim()));
		}
	}

	/// <summary>
	/// Parses the <see cref="IEnumerable{SemanticVersionRange}"/> provided into a <see cref="SemanticVersionRangeSet"/>
	/// </summary>
	/// <param name="semanticVersionRanges">The <see cref="IEnumerable{SemanticVersionRange}"/> to parse into a <see cref="SemanticVersionRangeSet"/></param>
	public SemanticVersionRangeSet(IEnumerable<SemanticVersionRange> semanticVersionRanges) {
		this.semanticVersionRanges = semanticVersionRanges.ToList();
	}

	private readonly List<SemanticVersionRange> semanticVersionRanges;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns>
	/// <inheritdoc/>
	/// </returns>
	override public string ToString() {
		string str = string.Empty;
		foreach (SemanticVersionRange semanticVersionRange in semanticVersionRanges) {
			str += semanticVersionRange.ToString() + "||";
		}
		return str[..^2];
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns>
	/// <inheritdoc/>
	/// </returns>
	public override int GetHashCode() {
		int sum = 0;
		foreach (SemanticVersionRange semanticVersionRange in semanticVersionRanges)
			sum += semanticVersionRange.GetHashCode();
		return sum;
	}

	/// <summary>
	/// Checks if a <see cref="SemanticVersion"/> is included in this <see cref="SemanticVersionRangeSet"/>
	/// </summary>
	/// <param name="semanticVersion">The <see cref="SemanticVersion"/> to check</param>
	/// <returns>Whether or not <paramref name="semanticVersion"/> is included in this <see cref="SemanticVersionRangeSet"/></returns>
	public bool Includes(SemanticVersion semanticVersion) {
		foreach (SemanticVersionRange semanticVersionRange in semanticVersionRanges) {
			if (semanticVersionRange.Includes(semanticVersion)) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="other"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public bool Equals(SemanticVersionRangeSet? other) {
		if (other is null) {
			return false;
		}
		foreach (SemanticVersionRange semanticVersionRange in semanticVersionRanges) {
			if (!other.semanticVersionRanges.Contains(semanticVersionRange)) {
				return false;
			}
		}
		foreach (SemanticVersionRange semanticVersionRange in other.semanticVersionRanges) {
			if (!semanticVersionRanges.Contains(semanticVersionRange)) {
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="obj"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public bool Equals(object? obj) => obj is SemanticVersionRangeSet semanticVersionRangeSet && Equals(semanticVersionRangeSet);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public static bool operator ==(SemanticVersionRangeSet left, SemanticVersionRangeSet right) => left is null ? right is null : left.Equals(right);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public static bool operator !=(SemanticVersionRangeSet left, SemanticVersionRangeSet right) => !(left == right);
}
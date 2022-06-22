using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Utils;

/// <summary>
/// A class that represents a <a href="https://semver.org">semantic version</a>
/// </summary>
public class SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion> {
	private static readonly Regex REGEX = new Regex(@"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$");

	/// <summary>
	/// Creates a new <see cref="SemanticVersion"/> with the specified values for it's properties
	/// </summary>
	/// <param name="major">The major version</param>
	/// <param name="minor">The minor version</param>
	/// <param name="patch">The patch version</param>
	/// <param name="preRelease">The pre-release</param>
	/// <param name="buildMetadata">The build metadata</param>
	public SemanticVersion(BigInteger major, BigInteger minor, BigInteger patch, string preRelease = null, string buildMetadata = null) {
		this.major = major;
		this.minor = minor;
		this.patch = patch;
		this.preRelease = preRelease ?? string.Empty;
		this.buildMetadata = buildMetadata ?? string.Empty;
	}

	/// <summary>
	/// Parses the <see cref="string"/> provided in a <see cref="SemanticVersion"/>
	/// </summary>
	/// <param name="raw">The <see cref="string"/> to parse into a <see cref="SemanticVersion"/></param>
	public SemanticVersion(string raw) {
		Match match = REGEX.Match(raw);
		if (match.Value != raw)
			throw new InvalidSemanticVersionException($"Invalid SemanticVersion: \"{raw}\"");
		
		major = BigInteger.Parse(match.Groups["major"].Value);
		minor = BigInteger.Parse(match.Groups["minor"].Value);
		patch = BigInteger.Parse(match.Groups["patch"].Value);
		preRelease = match.Groups["prerelease"].Value;
		buildMetadata = match.Groups["buildmetadata"].Value;
	}

	/// <summary>
	/// An <see cref="Exception"/> that is thrown when an invalid <see cref="SemanticVersion"/> is initiaized
	/// </summary>
	public class InvalidSemanticVersionException : Exception {
		internal InvalidSemanticVersionException(string message) : base(message) {}
	}

	/// <summary>
	/// The major version
	/// </summary>
	public BigInteger major { get; set; }

	/// <summary>
	/// The minor version
	/// </summary>
	public BigInteger minor { get; set; }

	/// <summary>
	/// The patch version
	/// </summary>
	public BigInteger patch { get; set; }

	/// <summary>
	/// The pre-release
	/// </summary>
	public string preRelease { get; set; }

	/// <summary>
	/// The build metadata
	/// </summary>
	public string buildMetadata { get; set; }

	/// <summary>
	/// Creates an instance of <see cref="ReadOnlySemanticVersion"/> with the data of this instance and returns it
	/// </summary>
	/// <returns>A new <see cref="ReadOnlySemanticVersion"/> that has the same data as this instance</returns>
	public ReadOnlySemanticVersion ToReadOnly() => new ReadOnlySemanticVersion(major, minor, patch, preRelease, buildMetadata);

	/// <summary>
	/// Creates a deep copy of this <see cref="SemanticVersion"/>
	/// </summary>
	/// <returns>A deep copy of this <see cref="SemanticVersion"/></returns>
	public SemanticVersion Clone() => new SemanticVersion(major, minor, patch, preRelease, buildMetadata);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="other"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public bool Equals(SemanticVersion other) => other is not null && other.major == major && other.minor == minor && other.patch == patch && other.preRelease == preRelease;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="obj"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	override public bool Equals(object obj) => obj is SemanticVersion semanticVersion && Equals(semanticVersion);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public int GetHashCode() => ToString().GetHashCode();

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <returns><inheritdoc/></returns>
	override public string ToString() {
		string str = $"{major}.{minor}.{patch}";
		if (preRelease != string.Empty)
			str += $"-{preRelease}";
		if (buildMetadata != string.Empty)
			str += $"+{buildMetadata}";
		return str;
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="other"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public int CompareTo(SemanticVersion other) {
		if (other is null)
			return 1;

		if (major > other.major)
			return 1;
		else if (major < other.major)
			return -1;

		if (minor > other.minor)
			return 1;
		else if (minor < other.minor)
			return -1;

		if (patch > other.patch)
			return 1;
		else if (patch < other.patch)
			return -1;

		//if left has preRelease and right doesn't
		//right is bigger than left
		if (preRelease != string.Empty && other.preRelease == string.Empty)
			return -1;

		//if right has preRelease and left doesn't
		//left is bigger than right
		if (other.preRelease != string.Empty && preRelease == string.Empty)
			return 1;

		if (other.preRelease != string.Empty && preRelease != string.Empty) {
			string[] leftIdentifiers = preRelease.Split(".");
			string[] rightIdentifiers = other.preRelease.Split(".");
			int n = Math.Max(leftIdentifiers.Length, rightIdentifiers.Length);
			for (int i = 0; i < n; i += 1) {
				string leftIdentifier;
				string rightIdentifier;
				try {
					leftIdentifier = leftIdentifiers[i];
					rightIdentifier = rightIdentifiers[i];
				} catch (IndexOutOfRangeException) {
					//one of them has more identifiers than the other and that is what decides precedence,
					//making the one with more identifiers "bigger", because all other ones are identical
					if (leftIdentifiers.Length < rightIdentifiers.Length)
						return -1;
					else//acts as "if (leftIdentifiers.Length > rightIdentifiers.Length)" since if they're the same length it doesn't get in this catch block and if its smaller it gets caught by the previous if statement
						return 1;
				}

				leftIdentifier = leftIdentifiers[i];
				rightIdentifier = rightIdentifiers[i];

				bool leftInt;//if left's identifier is a number
				bool rightInt;//if right's identifier is a number
				try {
					int temp = Convert.ToInt32(leftIdentifier);
					leftInt = true;
				} catch (FormatException) {
					leftInt = false;
				}
				try {
					int temp = Convert.ToInt32(rightIdentifier);
					rightInt = true;
				} catch (FormatException) {
					rightInt = false;
				}

				//if left's identifier is a number and right's isn't
				if (leftInt && !rightInt)
					return -1;

				//if left's identifier is not a number and right's is
				if (!leftInt && rightInt)
					return 1;

				//if both are numbers
				if (leftInt && rightInt) {
					BigInteger l = BigInteger.Parse(leftIdentifier);
					BigInteger r = BigInteger.Parse(rightIdentifier);
					if (l < r)
						return -1;
					else if (l > r)
						return 1;
				}

				//if none are numbers
				if (!leftInt && !rightInt) {
					try {
						//the SemanticVersion with the last identifier alphabetically is the bigger one
						for (int j = 0; j < Math.Max(leftIdentifier.Length, rightIdentifier.Length); j += 1) {
							if (leftIdentifier[j] < rightIdentifier[j])
								return -1;
							if (leftIdentifier[j] > rightIdentifier[j])
								return 1;
						}
					} catch (IndexOutOfRangeException) {
						//the larger identifier corresponds to the bigger SemanticVersion since null is smaller than every other char
						if (leftIdentifier.Length < rightIdentifier.Length)
							return -1;
						else//acts as "if (leftIdentifier.Length > rightIdentifier.Length)" since if they're the same length it doesn't get in this catch block and if its smaller it gets caught by the previous if statement
							return 1;
					}
				}
			}
		}

		//if all the above failed that means they're equal (excluding build metadata)
		return 0;
	}

	/// <summary>
	/// Gets the <see cref="SemanticVersion"/> immediatly following this one
	/// </summary>
	/// <returns>The <see cref="SemanticVersion"/> immediatly following this one</returns>
	public SemanticVersion Next() => preRelease == string.Empty ? new SemanticVersion(major, minor, patch + 1, "0") : new SemanticVersion(major, minor, patch, preRelease + ".0");
	
	/// <summary>
	/// Implicitly casts the <see cref="SemanticVersion"/> into a <see cref="string"/>
	/// </summary>
	/// <param name="semanticVersion">The <see cref="SemanticVersion"/> to cast</param>
	public static implicit operator string(SemanticVersion semanticVersion) => semanticVersion.ToString();

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public static bool operator ==(SemanticVersion left, SemanticVersion right) => left is null ? right is null : left.Equals(right);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public static bool operator !=(SemanticVersion left, SemanticVersion right) => !(left == right);

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public static bool operator <(SemanticVersion left, SemanticVersion right) => left is null || left.CompareTo(right) == -1;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public static bool operator >(SemanticVersion left, SemanticVersion right) => left is not null && left.CompareTo(right) == 1;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public static bool operator <=(SemanticVersion left, SemanticVersion right) => left == right || left < right;

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	/// <param name="left"><inheritdoc/></param>
	/// <param name="right"><inheritdoc/></param>
	/// <returns><inheritdoc/></returns>
	public static bool operator >=(SemanticVersion left, SemanticVersion right) => left == right || left > right;
}
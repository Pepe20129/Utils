using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Utils;

/// <summary>
/// A class that represents an <a href="https://github.com/npm/node-semver/blob/main/README.md#ranges">npm range of semantic versions</a>, (not a set of ranges)<br/>
/// If there's differing behaviour between this implementation and the spec, it is a bug, please report it
/// </summary>
public class SemanticVersionRange : IEquatable<SemanticVersionRange> {
	/// <summary>
	/// Creates a new <see cref="SemanticVersionRange"/> with the specified values for it's properties
	/// </summary>
	/// <param name="min"><inheritdoc cref="min"/></param>
	/// <param name="minActive"><inheritdoc cref="minActive"/></param>
	/// <param name="max"><inheritdoc cref="max"/></param>
	/// <param name="maxActive"><inheritdoc cref="maxActive"/></param>
	/// <param name="includePrereleases"><inheritdoc cref="includePrereleases"/></param>
	public SemanticVersionRange(SemanticVersion? min, bool minActive, SemanticVersion? max, bool maxActive, bool includePrereleases) {
		this.min = min;
		this.minActive = minActive;
		this.max = max;
		this.maxActive = maxActive;
		this.includePrereleases = includePrereleases;
	}

	/// <summary>
	/// Parses the <see cref="string"/> provided into a <see cref="SemanticVersionRange"/>
	/// </summary>
	/// <param name="raw">The <see cref="string"/> to parse into a <see cref="SemanticVersionRange"/></param>
	public SemanticVersionRange(string raw) : this(raw, false) {}

	/// <summary>
	/// Parses the <see cref="string"/> provided into a <see cref="SemanticVersionRange"/>
	/// </summary>
	/// <param name="raw">The <see cref="string"/> to parse into a <see cref="SemanticVersionRange"/></param>
	/// <param name="includePrereleases"><inheritdoc cref="includePrereleases"/></param>
	public SemanticVersionRange(string raw, bool includePrereleases) {
		this.raw = raw;
		this.includePrereleases = includePrereleases;

		if (raw == string.Empty || raw == "*" || raw == "x" || raw == "X") {
			min = null;
			max = null;
			minActive = false;
			maxActive = false;
			return;
		}


		Match match;

		if (raw.StartsWith("=")) {
			match = PARTIAL_REGEX.Match(raw.Remove(0, 1));
			if (match.Value == raw.Remove(0, 1)) {
				SemanticVersion semanticVersion = new SemanticVersion(SemverStringFromPartialMatch(match));

				min = semanticVersion;
				max = semanticVersion.Next();
				minActive = true;
				maxActive = true;
				return;
			}
		}
		
		if (raw.StartsWith(">=")) {
			match = PARTIAL_REGEX.Match(raw.Remove(0, 2));
			if (match.Value == raw.Remove(0, 2)) {
				SemanticVersion semanticVersion = new SemanticVersion(SemverStringFromPartialMatch(match));

				min = semanticVersion;
				max = null;
				minActive = true;
				maxActive = false;
				return;
			}
		}

		if (raw.StartsWith(">")) {
			match = PARTIAL_REGEX.Match(raw.Remove(0, 1));
			if (match.Value == raw.Remove(0, 1)) {
				SemanticVersion semanticVersion = new SemanticVersion(SemverStringFromPartialMatch(match));

				min = semanticVersion.Next();
				max = null;
				minActive = true;
				maxActive = false;
				return;
			}
		}

		if (raw.StartsWith("<=")) {
			match = PARTIAL_REGEX.Match(raw.Remove(0, 2));
			if (match.Value == raw.Remove(0, 2)) {
				SemanticVersion semanticVersion = new SemanticVersion(SemverStringFromPartialMatch(match));

				min = null;
				max = semanticVersion.Next();
				minActive = false;
				maxActive = true;
				return;
			}
		}

		if (raw.StartsWith("<")) {
			match = PARTIAL_REGEX.Match(raw.Remove(0, 1));
			if (match.Value == raw.Remove(0, 1)) {
				SemanticVersion semanticVersion = new SemanticVersion(SemverStringFromPartialMatch(match));

				min = null;
				max = semanticVersion;
				minActive = false;
				maxActive = true;
				return;
			}
		}

		if (raw.StartsWith("~")) {
			match = PARTIAL_REGEX.Match(raw.Remove(0, 1));
			if (match.Value == raw.Remove(0, 1)) {
				SemanticVersion semanticVersion = new SemanticVersion(SemverStringFromPartialMatch(match));
				min = semanticVersion.Clone();
				if (match.Groups["minor"].Success) {
					semanticVersion = new SemanticVersion($"{match.Groups["major"].Value}.{BigInteger.Parse(match.Groups["minor"].Value) + 1}.0-0");
				} else {
					semanticVersion = new SemanticVersion($"{BigInteger.Parse(match.Groups["major"].Value) + 1}.0.0-0");
				}
				max = semanticVersion;
				minActive = true;
				maxActive = true;
				return;
			}
		}
		
		if (raw.StartsWith("^")) {
			SemanticVersion semanticVersion;
			match = PARTIAL_REGEX.Match(raw.Remove(0, 1));
			if (match.Value == raw.Remove(0, 1)) {
				semanticVersion = new SemanticVersion(SemverStringFromPartialMatch(match));
				min = semanticVersion.Clone();
				if (!match.Groups["patch"].Success) {
					if (match.Groups["minor"].Success) {
						if (semanticVersion.major != 0) {
							semanticVersion.major += 1;
							semanticVersion.minor = 0;
							semanticVersion.patch = 0;
							semanticVersion.preRelease = "0";
						} else {
							semanticVersion.minor += 1;
							semanticVersion.patch = 0;
							semanticVersion.preRelease = "0";
						}
					} else {
						semanticVersion.major += 1;
						semanticVersion.minor = 0;
						semanticVersion.patch = 0;
						semanticVersion.preRelease = "0";
					}
				} else {
					if (semanticVersion.major == 0) {
						semanticVersion.minor += 1;
						semanticVersion.patch = 0;
					} else {
						semanticVersion.major += 1;
						semanticVersion.minor = 0;
						semanticVersion.patch = 0;
					}
					semanticVersion.preRelease = "0";
				}
				max = semanticVersion;
				minActive = true;
				maxActive = true;
				return;
			}

			match = X_REGEX.Match(raw.Remove(0, 1));
			if (match.Value == raw.Remove(0, 1)) {
				semanticVersion = new SemanticVersion(SemverStringFromXMatch(match));
				min = semanticVersion.Clone();
				if (!match.Groups["patch"].Success || match.Groups["patch"].Value == "x" || match.Groups["patch"].Value == "X" || match.Groups["patch"].Value == "*") {
					if (!(!match.Groups["minor"].Success || match.Groups["minor"].Value == "x" || match.Groups["minor"].Value == "X" || match.Groups["minor"].Value == "*")) {
						if (semanticVersion.major != 0) {
							semanticVersion.major += 1;
							semanticVersion.minor = 0;
							semanticVersion.patch = 0;
							semanticVersion.preRelease = "0";
						} else {
							semanticVersion.minor += 1;
							semanticVersion.patch = 0;
							semanticVersion.preRelease = "0";
						}
					} else {
						semanticVersion.major += 1;
						semanticVersion.minor = 0;
						semanticVersion.patch = 0;
						semanticVersion.preRelease = "0";
					}
				}
				max = semanticVersion;
				minActive = true;
				maxActive = true;
				return;
			}
		}

		match = PARTIAL_REGEX.Match(raw);
		if (match.Value == raw) {
			SemanticVersion semanticVersion = new SemanticVersion(SemverStringFromPartialMatch(match));

			min = semanticVersion;
			max = semanticVersion.Next();
			minActive = true;
			maxActive = true;
			return;
		}

		match = X_REGEX.Match(raw);
		if (match.Value == raw) {
			bool p = match.Groups["patch"].Value == "x" || match.Groups["patch"].Value == "X" || match.Groups["patch"].Value == "*";
			bool m = match.Groups["minor"].Value == "x" || match.Groups["minor"].Value == "X" || match.Groups["minor"].Value == "*";

			SemanticVersion semanticVersion = new SemanticVersion(SemverStringFromXMatch(match));
			min = semanticVersion.Clone();
			if (m)
				semanticVersion.major += 1;
			else if (p)
				semanticVersion.minor += 1;
			semanticVersion.preRelease = "0";
			max = semanticVersion;
			minActive = true;
			maxActive = true;
			return;
		}

		match = PARTIAL_HYPHEN_REGEX.Match(raw);
		if (match.Value == raw) {
			string minVersionString = $"{match.Groups["major1"].Value}.{(match.Groups["minor1"].Success ? match.Groups["minor1"].Value : 0)}.{(match.Groups["patch1"].Success ? match.Groups["patch1"].Value : 0)}";
			if (match.Groups["prerelease1"].Success)
				minVersionString += $"-{match.Groups["prerelease1"].Value}";
			min = new SemanticVersion(minVersionString);
			string maxVersionString = $"{match.Groups["major2"].Value}.{(match.Groups["minor2"].Success ? match.Groups["minor2"].Value : 0)}.{(match.Groups["patch2"].Success ? match.Groups["patch2"].Value : 0)}";
			if (match.Groups["prerelease2"].Success)
				maxVersionString += $"-{match.Groups["prerelease2"].Value}";
			max = new SemanticVersion(maxVersionString).Next();
			minActive = true;
			maxActive = true;
			return;
		}

		throw new InvalidSemanticVersionRangeException($"Invalid SemanticVersionRange: \"{raw}\"");
	}

	private static string SemverStringFromPartialMatch(Match match) {
		string str = match.Groups["major"].Value + ".";

		string minor = "0";
		if (match.Groups["minor"].Success)
			minor = match.Groups["minor"].Value;
		
		str += minor + ".";

		string patch = "0";
		if (match.Groups["patch"].Success)
			patch = match.Groups["patch"].Value;
		
		str += patch;

		if (match.Groups["prerelease"].Success)
			str += "-" + match.Groups["prerelease"].Value;
		
		if (match.Groups["buildmetadata"].Success)
			str += "+" + match.Groups["buildmetadata"].Value;
		
		return str;
	}

	private static string SemverStringFromXMatch(Match match) {
		string str = match.Groups["major"].Value + ".";

		string minor = "0";
		if (!(match.Groups["minor"].Value == "x" || match.Groups["minor"].Value == "X" || match.Groups["minor"].Value == "*"))
			minor = match.Groups["minor"].Value;

		str += minor + ".";

		string patch = "0";

		str += patch;

		if (match.Groups["prerelease"].Success)
			str += "-" + match.Groups["prerelease"].Value;

		if (match.Groups["buildmetadata"].Success)
			str += "+" + match.Groups["buildmetadata"].Value;

		return str;
	}

	private static bool HaveSameTuple(SemanticVersion semanticVersion1, SemanticVersion semanticVersion2) => semanticVersion1.major == semanticVersion2.major && semanticVersion1.minor == semanticVersion2.minor && semanticVersion1.patch == semanticVersion2.patch;
	
	/// <summary>
	/// An <see cref="Exception"/> that is thrown when an invalid <see cref="SemanticVersionRange"/> is initialized
	/// </summary>
	public class InvalidSemanticVersionRangeException : Exception {
		internal InvalidSemanticVersionRangeException(string message) : base(message) {}
	}

	/// <summary>
	/// Checks if a <see cref="SemanticVersion"/> is included in this <see cref="SemanticVersionRange"/>
	/// </summary>
	/// <param name="semanticVersion">The <see cref="SemanticVersion"/> to check</param>
	/// <returns>Whether or not <paramref name="semanticVersion"/> is included in this <see cref="SemanticVersionRange"/></returns>
	public bool Includes(SemanticVersion semanticVersion) {
		if (semanticVersion is null || (minActive && semanticVersion < min!) || (maxActive && semanticVersion >= max!)) {
			return false;
		}
		//if we don't include pre-releases of a different [major, minor, patch] tuple and the version specified has a pre-release
		if (!includePrereleases && semanticVersion.preRelease != string.Empty) {
			//if (the version specified has (a different tuple from min) and (a different tuple from max)) or
			//(it has the same tuple as one of them and that one doesn't have a pre-release), return false
			//                           ^_______________|
			if (((!minActive || !HaveSameTuple(semanticVersion, min!)) && (!maxActive || !HaveSameTuple(semanticVersion, max!))) ||
				(minActive && HaveSameTuple(semanticVersion, min!) && (min!.preRelease == string.Empty || min!.preRelease == "0")) ||
				(maxActive && HaveSameTuple(semanticVersion, max!) && (max!.preRelease == string.Empty || max!.preRelease == "0"))) {
				return false;
			} else {
				//this only happens if the version specified has a pre-release and the same [major, minor, patch] tuple as one of the limit ones which also has a pre-release
				return true;
			}
		}
		return true;
	}

	/// <inheritdoc/>
	override public bool Equals(object? obj) => obj is SemanticVersionRange semanticVersionRange && Equals(semanticVersionRange);

	/// <inheritdoc/>
	public bool Equals(SemanticVersionRange? other) => other is not null && other.min == min && other.minActive == minActive && other.max == max && other.maxActive == maxActive && other.includePrereleases == includePrereleases;

	/// <inheritdoc/>
	override public int GetHashCode() => ToString().GetHashCode();

	/// <inheritdoc/>
	override public string ToString() {
		if (raw != null) {
			return raw;
		}
		if (minActive) {
			if (maxActive) {
				SemanticVersion semVer = max!.Clone();
				semVer.preRelease = string.Empty;
				semVer.patch -= 1;
				return $"{min} - {((max.preRelease == "0" && max.patch > 0) ? semVer : throw new NotImplementedException())}";
			} else {
				return $">={min}";
			}
		} else {
			if (maxActive) {
				return $"<{max}";
			} else {
				return "*";
			}
		}
	}

	/// <summary>
	/// Creates a deep copy of the object
	/// </summary>
	/// <returns>A deep copy of the object</returns>
	public SemanticVersionRange Clone() => new SemanticVersionRange(min, minActive, max, maxActive, includePrereleases);
	
	private static readonly Regex PARTIAL_REGEX = new Regex(@"^(?<major>0|[1-9]\d*)(?:\.(?<minor>0|[1-9]\d*)(?:\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?)?)?$");
	private static readonly Regex X_REGEX = new Regex(@"^(?<major>0|[1-9]\d*)\.(?:(?<minor>[xX*])(?:\.(?<patch>[xX*]))?|(?<minor>0|[1-9]\d*)(?:\.(?<patch>[xX*])))(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$");
	private static readonly Regex PARTIAL_HYPHEN_REGEX = new Regex(@"^(?<major1>0|[1-9]\d*)(?:\.(?<minor1>0|[1-9]\d*)(?:\.(?<patch1>0|[1-9]\d*)(?:-(?<prerelease1>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata1>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?)?)? - (?<major2>0|[1-9]\d*)(?:\.(?<minor2>0|[1-9]\d*)(?:\.(?<patch2>0|[1-9]\d*)(?:-(?<prerelease2>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata2>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?)?)?$");

	private readonly string? raw;

	/// <summary>
	/// The minimum (inclusive) version that a <see cref="SemanticVersion"/> must be to be included in this <see cref="SemanticVersionRange"/>
	/// </summary>
	public SemanticVersion? min { get; set; }

	/// <summary>
	/// Whether <see cref="min"/> is active or not
	/// </summary>
	public bool minActive { get; set; }

	/// <summary>
	/// The maximum (exclusive) version that a <see cref="SemanticVersion"/> must be to be included in this <see cref="SemanticVersionRange"/>
	/// </summary>
	public SemanticVersion? max { get; set; }

	/// <summary>
	/// Whether <see cref="max"/> is active or not
	/// </summary>
	public bool maxActive { get; set; }

	/// <summary>
	/// Whether pre-releases of a different [major, minor, patch] tuple should be included in the range or not (not yet completly implemented)
	/// </summary>
	public bool includePrereleases { get; set; }

	/// <inheritdoc/>
	public static bool operator ==(SemanticVersionRange left, SemanticVersionRange right) => left is null ? right is null : left.Equals(right);

	/// <inheritdoc/>
	public static bool operator !=(SemanticVersionRange left, SemanticVersionRange right) => !(left == right);
}

using System.Numerics;

namespace Utils;

/// <summary>
/// An immutable variant of <see cref="SemanticVersion"/>
/// </summary>
public class ImmutableSemanticVersion : SemanticVersion {
	/// <inheritdoc cref="SemanticVersion(BigInteger, BigInteger, BigInteger, string, string)"/>
	public ImmutableSemanticVersion(BigInteger major, BigInteger minor, BigInteger patch, string? preRelease = null, string? buildMetadata = null) : base(major, minor, patch, preRelease, buildMetadata) {}

	/// <inheritdoc cref="SemanticVersion(string)"/>
	public ImmutableSemanticVersion(string raw) : base(raw) {}

	/// <inheritdoc cref="SemanticVersion.major"/>
	public new BigInteger major {
		get {
			return base.major;
		}
		init {
			base.major = value;
		}
	}

	/// <inheritdoc cref="SemanticVersion.minor"/>
	public new BigInteger minor {
		get {
			return base.minor;
		}
		init {
			base.minor = value;
		}
	}

	/// <inheritdoc cref="SemanticVersion.patch"/>
	public new BigInteger patch {
		get {
			return base.patch;
		}
		init {
			base.patch = value;
		}
	}

	/// <inheritdoc cref="SemanticVersion.preRelease"/>
	public new string preRelease {
		get {
			return base.preRelease;
		}
		init {
			base.preRelease = value;
		}
	}

	/// <inheritdoc cref="SemanticVersion.buildMetadata"/>
	public new string buildMetadata {
		get {
			return base.buildMetadata;
		}
		init {
			base.buildMetadata = value;
		}
	}
}
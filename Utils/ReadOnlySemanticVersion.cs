using System.Numerics;

namespace Utils;

/// <summary>
/// A readonly version of <see cref="SemanticVersion"/>
/// </summary>
public class ReadOnlySemanticVersion : SemanticVersion {
	/// <summary>
	/// <inheritdoc cref="SemanticVersion(BigInteger, BigInteger, BigInteger, string, string)"/>
	/// </summary>
	/// <param name="major">The major version</param>
	/// <param name="minor">The minor version</param>
	/// <param name="patch">The patch version</param>
	/// <param name="preRelease">The pre-release</param>
	/// <param name="buildMetadata">The build metadata</param>
	public ReadOnlySemanticVersion(BigInteger major, BigInteger minor, BigInteger patch, string preRelease = null, string buildMetadata = null) : base(major, minor, patch, preRelease, buildMetadata) {}

	/// <summary>
	/// <inheritdoc cref="SemanticVersion(string)"/>
	/// </summary>
	/// <param name="raw"><inheritdoc cref="SemanticVersion(string)"/></param>
	public ReadOnlySemanticVersion(string raw) : base(raw) {}

	/// <summary>
	/// <inheritdoc cref="SemanticVersion.major"/>
	/// </summary>
	public new BigInteger major {
		get {
			return base.major;
		}
		init {
			base.major = value;
		}
	}

	/// <summary>
	/// <inheritdoc cref="SemanticVersion.minor"/>
	/// </summary>
	public new BigInteger minor {
		get {
			return base.minor;
		}
		init {
			base.minor = value;
		}
	}

	/// <summary>
	/// <inheritdoc cref="SemanticVersion.patch"/>
	/// </summary>
	public new BigInteger patch {
		get {
			return base.patch;
		}
		init {
			base.patch = value;
		}
	}

	/// <summary>
	/// <inheritdoc cref="SemanticVersion.preRelease"/>
	/// </summary>
	public new string preRelease {
		get {
			return base.preRelease;
		}
		init {
			base.preRelease = value;
		}
	}

	/// <summary>
	/// <inheritdoc cref="SemanticVersion.buildMetadata"/>
	/// </summary>
	public new string buildMetadata {
		get {
			return base.buildMetadata;
		}
		init {
			base.buildMetadata = value;
		}
	}
}
using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.Sensor
{
    // typedef struct _k4a_version_t
    // {
    //     uint32_t major;
    //     uint32_t minor;
    //     uint32_t iteration;
    // } k4a_version_t;
    //
    /// <summary>Version information about sensor firmware.</summary>
    /// <remarks>Can be smoothly converted to/from <see cref="Version"/> object for convenience of usage in your code.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct FirmwareVersion :
        IEquatable<FirmwareVersion>, IEquatable<Version>,
        IComparable<FirmwareVersion>, IComparable<Version>, IComparable,
        IFormattable
    {
        /// <summary>Major version; represents a breaking change.</summary>
        public int Major;

        /// <summary>Minor version; represents additional features, no regression from lower versions with same major version.</summary>
        public int Minor;

        /// <summary>Reserved.</summary>
        public int Revision;

        /// <summary>Creates version with specified components.</summary>
        /// <param name="major">Value for field <see cref="Major"/>.</param>
        /// <param name="minor">Value for field <see cref="Minor"/>.</param>
        /// <param name="revision">Value for field <see cref="Revision"/>.</param>
        public FirmwareVersion(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        /// <summary>Creates version from <see cref="Version"/> object.</summary>
        /// <param name="version">Version. Not <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="version"/> cannot be <see langword="null"/>.</exception>
        public FirmwareVersion(Version version)
        {
            if (version is null)
                throw new ArgumentNullException(nameof(version));
            Major = version.Major;
            Minor = version.Minor;
            Revision = version.Build;
        }

        /// <summary>Explicitly converts to standard <see cref="Version"/> object.</summary>
        /// <returns>Corresponding <see cref="Version"/> object. Not <see langword="null"/>.</returns>
        public Version ToVersion()
            => new(Major, Minor, Revision);

        /// <summary>Implicit conversion to <see cref="Version"/>.</summary>
        /// <param name="version">Firmware version to be converted to <see cref="Version"/>.</param>
        /// <seealso cref="ToVersion"/>
        public static implicit operator Version(FirmwareVersion version)
            => version.ToVersion();

        /// <summary>Implicit conversion from <see cref="Version"/>.</summary>
        /// <param name="version">Version to be converted to <see cref="FirmwareVersion"/>. Not <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="version"/> cannot be <see langword="null"/>.</exception>
        public static implicit operator FirmwareVersion(Version version)
            => new(version);

        /// <summary>Per-component comparison of versions. Implementation of <see cref="IEquatable{FirmwareVersion}"/>.</summary>
        /// <param name="other">Version to be compared with this one.</param>
        /// <returns><c>true</c> - versions are the same, <c>false</c> - otherwise.</returns>
        public bool Equals(FirmwareVersion other)
            => Major.Equals(other.Major) && Minor.Equals(other.Minor) && Revision.Equals(other.Revision);

        /// <summary>Per-component comparison of versions. Implementation of <see cref="IEquatable{Version}"/>.</summary>
        /// <param name="other">Version to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if versions are the same, <see langword="false"/> - otherwise.</returns>
        public bool Equals(Version? other)
            => other is not null && Equals(new FirmwareVersion(other));

        /// <summary>Per-component comparison of versions.</summary>
        /// <param name="obj">Object to be compared with this one.</param>
        /// <returns><see langword="true"/> - if <paramref name="obj"/> is not <see langword="null"/> and it is version and versions are equal, <see langword="false"/> - otherwise.</returns>
        public override bool Equals(object? obj)
            => obj switch
            {
                null => false,
                FirmwareVersion fv => Equals(fv),
                Version v => Equals(v),
                _ => false
            };

        /// <summary>To be consistent with <see cref="Equals(FirmwareVersion)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(FirmwareVersion)"/>
        public static bool operator ==(FirmwareVersion left, FirmwareVersion right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(FirmwareVersion)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(FirmwareVersion)"/>
        public static bool operator !=(FirmwareVersion left, FirmwareVersion right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Version)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Version)"/>
        public static bool operator ==(FirmwareVersion left, Version? right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Version)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Version)"/>
        public static bool operator !=(FirmwareVersion left, Version? right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Version)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Version)"/>
        public static bool operator ==(Version? left, FirmwareVersion right)
            => right.Equals(left);

        /// <summary>To be consistent with <see cref="Equals(Version)"/>.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Version)"/>
        public static bool operator !=(Version? left, FirmwareVersion right)
            => !right.Equals(left);

        /// <summary>Calculates hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        public override int GetHashCode()
            => unchecked(Major * 10000 + Minor * 100 + Revision);

        /// <summary>Versions comparison. Implementation of <see cref="IComparable{FirmwareVersion}"/>.</summary>
        /// <param name="other">Other version to be compared with this one.</param>
        /// <returns>
        /// <c>1</c> - <paramref name="other"/> is less than this one,
        /// <c>0</c> - <paramref name="other"/> equals this one,
        /// <c>-1</c> - <paramref name="other"/> is greater than this one.
        /// </returns>
        public int CompareTo(FirmwareVersion other)
        {
            var res = Major.CompareTo(other.Major);
            if (res != 0)
                return res;
            res = Minor.CompareTo(other.Minor);
            if (res != 0)
                return res;
            return Revision.CompareTo(other.Revision);
        }

        /// <summary>Versions comparison. Implementation of <see cref="IComparable{Version}"/>.</summary>
        /// <param name="other">Other version to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns>
        /// <c>1</c> - <paramref name="other"/> is less than this one or is <see langword="null"/>,
        /// <c>0</c> - <paramref name="other"/> equals this one,
        /// <c>-1</c> - <paramref name="other"/> is less than this one.
        /// </returns>
        public int CompareTo(Version? other)
            => other is null ? 1 : CompareTo(new FirmwareVersion(other));

        /// <summary>Versions comparison. Implementation of <see cref="IComparable{Version}"/>.</summary>
        /// <param name="obj">Other version to be compared with this one. Can be <see langword="null"/>.</param>
        /// <returns>
        /// <c>1</c> - <paramref name="obj"/> is greater than this one or is <see langword="null"/>,
        /// <c>0</c> - <paramref name="obj"/> equals this one,
        /// <c>-1</c> - <paramref name="obj"/> is less than this one.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is not comparable with this one.</exception>
        public int CompareTo(object? obj)
            => obj switch
            {
                null => 1,
                FirmwareVersion fv => CompareTo(fv),
                Version v => CompareTo(v),
                _ => throw new ArgumentException("Object is not a FirmwareVersion or Version", nameof(obj))
            };

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        public static bool operator <(FirmwareVersion left, FirmwareVersion right)
            => left.CompareTo(right) < 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or is equal to <paramref name="right"/>.</returns>
        public static bool operator <=(FirmwareVersion left, FirmwareVersion right)
            => left.CompareTo(right) <= 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        public static bool operator >(FirmwareVersion left, FirmwareVersion right)
            => left.CompareTo(right) > 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or is equal to <paramref name="right"/>.</returns>
        public static bool operator >=(FirmwareVersion left, FirmwareVersion right)
            => left.CompareTo(right) >= 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        public static bool operator <(FirmwareVersion left, Version? right)
            => left.CompareTo(right) < 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or is equal to <paramref name="right"/>.</returns>
        public static bool operator <=(FirmwareVersion left, Version? right)
            => left.CompareTo(right) <= 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/> or <paramref name="right"/> is <see langword="null"/>.</returns>
        public static bool operator >(FirmwareVersion left, Version? right)
            => left.CompareTo(right) > 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator. Can be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or is equal to <paramref name="right"/> or <paramref name="right"/> is <see langword="null"/>.</returns>
        public static bool operator >=(FirmwareVersion left, Version? right)
            => left.CompareTo(right) >= 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>..</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/> or <paramref name="left"/> is <see langword="null"/>.</returns>
        public static bool operator <(Version? left, FirmwareVersion right)
            => right.CompareTo(left) > 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>..</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or is equal to <paramref name="right"/> or <paramref name="left"/> is <see langword="null"/>.</returns>
        public static bool operator <=(Version? left, FirmwareVersion right)
            => right.CompareTo(left) >= 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>..</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        public static bool operator >(Version? left, FirmwareVersion right)
            => right.CompareTo(left) < 0;

        /// <summary>Versions comparison.</summary>
        /// <param name="left">Left part of operator. Can be <see langword="null"/>..</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or is equal to <paramref name="right"/>.</returns>
        public static bool operator >=(Version? left, FirmwareVersion right)
            => right.CompareTo(left) <= 0;

        /// <summary>String representation of version formatted using the specified format and provider.</summary>
        /// <param name="format">The format to use or <see langword="null"/> for default format.</param>
        /// <param name="formatProvider">The provider to use to format the value or <see langword="null"/> to obtain the numeric format information from the current locale setting.</param>
        /// <returns><c>{Major}.{Minor}.{Revision}</c></returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
            => $"{Major.ToString(format, formatProvider)}.{Minor.ToString(format, formatProvider)}.{Revision.ToString(format, formatProvider)}";

        /// <summary>String representation of version.</summary>
        /// <returns><c>{Major}.{Minor}.{Revision}</c></returns>
        public override string ToString()
            => $"{Major}.{Minor}.{Revision}";
    }
}

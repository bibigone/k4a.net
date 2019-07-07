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
    /// <summary>
    /// Version information about sensor firmware.
    /// </summary>
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

        public FirmwareVersion(Version version)
        {
            if (version is null)
                throw new ArgumentNullException(nameof(version));
            Major = version.Major;
            Minor = version.Minor;
            Revision = version.Build;
        }

        public Version ToVersion()
            => new Version(Major, Minor, Revision);

        public static implicit operator Version(FirmwareVersion version)
            => version.ToVersion();

        public static implicit operator FirmwareVersion(Version version)
            => new FirmwareVersion(version);

        /// <summary>Per-component comparison of versions.</summary>
        /// <param name="other">Version to be compared with this one.</param>
        /// <returns><c>true</c> - versions are the same, <c>false</c> - versions are differ from each other.</returns>
        public bool Equals(FirmwareVersion other)
            => Major.Equals(other.Major) && Minor.Equals(other.Minor) && Revision.Equals(other.Revision);

        public bool Equals(Version other)
            => Equals(new FirmwareVersion(other));

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is FirmwareVersion)
                return Equals((FirmwareVersion)obj);
            if (obj is Version)
                return Equals((Version)obj);
            return false;
        }

        public static bool operator ==(FirmwareVersion left, FirmwareVersion right)
            => left.Equals(right);

        public static bool operator !=(FirmwareVersion left, FirmwareVersion right)
            => !left.Equals(right);

        public static bool operator ==(FirmwareVersion left, Version right)
            => left.Equals(right);

        public static bool operator !=(FirmwareVersion left, Version right)
            => !left.Equals(right);

        public static bool operator ==(Version left, FirmwareVersion right)
            => new FirmwareVersion(left).Equals(right);

        public static bool operator !=(Version left, FirmwareVersion right)
            => !new FirmwareVersion(left).Equals(right);

        public override int GetHashCode()
            => unchecked(Major * 10000 + Minor * 100 + Revision);

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

        public int CompareTo(Version other)
            => CompareTo(new FirmwareVersion(other));

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;
            if (obj is FirmwareVersion)
                return CompareTo((FirmwareVersion)obj);
            if (obj is Version)
                return CompareTo((Version)obj);
            throw new ArgumentException("Object is not a FirmwareVersion or Version", nameof(obj));
        }

        public static bool operator <(FirmwareVersion left, FirmwareVersion right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(FirmwareVersion left, FirmwareVersion right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(FirmwareVersion left, FirmwareVersion right)
            => left.CompareTo(right) > 0;

        public static bool operator >=(FirmwareVersion left, FirmwareVersion right)
            => left.CompareTo(right) >= 0;

        public static bool operator <(FirmwareVersion left, Version right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(FirmwareVersion left, Version right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(FirmwareVersion left, Version right)
            => left.CompareTo(right) > 0;

        public static bool operator >=(FirmwareVersion left, Version right)
            => left.CompareTo(right) >= 0;

        public static bool operator <(Version left, FirmwareVersion right)
            => new FirmwareVersion(left).CompareTo(right) < 0;

        public static bool operator <=(Version left, FirmwareVersion right)
            => new FirmwareVersion(left).CompareTo(right) <= 0;

        public static bool operator >(Version left, FirmwareVersion right)
            => new FirmwareVersion(left).CompareTo(right) > 0;

        public static bool operator >=(Version left, FirmwareVersion right)
            => new FirmwareVersion(left).CompareTo(right) >= 0;

        public string ToString(string format, IFormatProvider formatProvider)
            => $"{Major.ToString(format, formatProvider)}.{Minor.ToString(format, formatProvider)}.{Revision.ToString(format, formatProvider)}";

        public override string ToString()
            => $"{Major}.{Minor}.{Revision}";
    }
}

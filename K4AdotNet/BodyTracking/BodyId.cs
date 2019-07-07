using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.BodyTracking
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BodyId :
        IEquatable<BodyId>, IEquatable<int>,
        IFormattable
    {
        public int Value;

        public BodyId(int value)
            => Value = value;

        public bool IsValid
            => Value != Invalid.Value;

        public bool Equals(BodyId other)
            => Value.Equals(other.Value);

        public bool Equals(int other)
            => Value.Equals(other);

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is BodyId)
                return Equals((BodyId)obj);
            if (obj is IConvertible)
                return Equals(Convert.ToInt32(obj));
            return false;
        }

        public static bool operator ==(BodyId left, BodyId right)
            => left.Equals(right);

        public static bool operator !=(BodyId left, BodyId right)
            => !left.Equals(right);

        public static bool operator ==(BodyId left, int right)
            => left.Equals(right);

        public static bool operator !=(BodyId left, int right)
            => !left.Equals(right);

        public static bool operator ==(int left, BodyId right)
            => left.Equals(right.Value);

        public static bool operator !=(int left, BodyId right)
            => !left.Equals(right.Value);

        public override int GetHashCode()
            => Value;

        public string ToString(string format, IFormatProvider formatProvider)
            => IsValid ? Value.ToString(format, formatProvider) : "INVALID";

        public override string ToString()
            => IsValid ? Value.ToString() : "INVALID";

        public static implicit operator int(BodyId id)
            => id.Value;

        public static implicit operator BodyId(int id)
            => new BodyId(id);

        // #define K4ABT_INVALID_BODY_ID 0xFFFFFFFF
        /// <summary>The invalid body id value.</summary>
        public static BodyId Invalid = new BodyId(-1);
    }
}

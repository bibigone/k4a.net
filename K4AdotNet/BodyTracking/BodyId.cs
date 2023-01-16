using System;
using System.Runtime.InteropServices;

namespace K4AdotNet.BodyTracking
{
    /// <summary>Tracked body ID.</summary>
    /// <remarks>
    /// Actually, this structure is an simple wrapper of <see cref="Int32"/> type.
    /// And <see cref="BodyId"/> value can be smoothly converted to/from <see cref="int"/> values for convenience of usage in your code.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct BodyId :
        IEquatable<BodyId>, IEquatable<int>,
        IFormattable
    {
        /// <summary>Value of ID. Should be of <see cref="uint"/> type but for CLS-compatibility it is declared is <see cref="int"/>.</summary>
        /// <remarks><para>
        /// There is special value:
        /// -1 (that is <see cref="uint.MaxValue"/>) - invalid body ID (see <see cref="Invalid"/>).
        /// </para><para>
        /// This structure is an wrapper around this value.
        /// </para></remarks>
        public int Value;

        /// <summary>Creates instance from integer value.</summary>
        /// <param name="value">Body ID.</param>
        public BodyId(int value)
            => Value = value;

        /// <summary>Is this ID valid?</summary>
        /// <seealso cref="Invalid"/>
        public bool IsValid
            => Value != Invalid.Value;

        /// <summary>Equality exactly like <see cref="int"/> type has.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(BodyId other)
            => Value.Equals(other.Value);

        /// <summary>Equality exactly like <see cref="int"/> type has.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(int other)
            => Value.Equals(other);

        /// <summary>Overloads <see cref="Object.Equals(object)"/> to be consistent with <see cref="Equals(BodyId)"/>.</summary>
        /// <param name="obj">Object to be compared with this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> can be cast to <see cref="BodyId"/> and result is equal to this one.</returns>
        public override bool Equals(object? obj)
            => obj switch
            {
                null => false,
                BodyId _ => Equals((BodyId)obj),
                IConvertible _ => Equals(Convert.ToInt32(obj)),
                _ => false
            };

        /// <summary>To be consistent with <see cref="Equals(BodyId)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(BodyId)"/>
        public static bool operator ==(BodyId left, BodyId right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(BodyId)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(BodyId)"/>
        public static bool operator !=(BodyId left, BodyId right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator ==(BodyId left, int right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator !=(BodyId left, int right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator ==(int left, BodyId right)
            => right.Equals(left);

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator !=(int left, BodyId right)
            => !right.Equals(left);

        /// <summary>Calculates hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        public override int GetHashCode()
            => Value;

        /// <summary>String representation of current instance.</summary>
        /// <param name="format">The format to use or <see langword="null"/> for default format.</param>
        /// <param name="formatProvider">The provider to use to format the value or <see langword="null"/> to obtain the numeric format information from the current locale setting.</param>
        /// <returns><c>{value}</c> or <c>INVALID</c> string.</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
            => IsValid ? Value.ToString(format, formatProvider) : "INVALID";

        /// <summary>String representation of current instance.</summary>
        /// <returns><c>{value}</c> or <c>INVALID</c> string.</returns>
        public override string ToString()
            => IsValid ? Value.ToString() : "INVALID";

        /// <summary>Implicit conversion to <see cref="int"/>.</summary>
        /// <param name="id">Value to be converted to <see cref="int"/>.</param>
        public static implicit operator int(BodyId id)
            => id.Value;

        /// <summary>Implicit conversion from <see cref="int"/>.</summary>
        /// <param name="id">Value to be converted to <see cref="BodyId"/>.</param>
        public static implicit operator BodyId(int id)
            => new(id);

        // #define K4ABT_INVALID_BODY_ID 0xFFFFFFFF
        /// <summary>The invalid body id value.</summary>
        public static BodyId Invalid = new(-1);
    }
}

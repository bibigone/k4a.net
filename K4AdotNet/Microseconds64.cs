using System;

namespace K4AdotNet
{
    /// <summary>64-bit time value in microseconds. Used for timestamps and delays.</summary>
    /// <remarks>
    /// Actually, this structure is an simple wrapper of <see cref="Int64"/> type.
    /// And <see cref="Microseconds64"/> value can be smoothly converted to/from <see cref="long"/> and <see cref="TimeSpan"/> values for convenience of usage in your code.
    /// </remarks>
    public struct Microseconds64 :
        IEquatable<Microseconds64>, IEquatable<TimeSpan>, IEquatable<long>,
        IComparable<Microseconds64>, IComparable<TimeSpan>, IComparable<long>, IComparable,
        IFormattable
    {
        /// <summary>Value in microseconds.</summary>
        /// <remarks>This structure is an wrapper around this value.</remarks>
        public long ValueUsec;

        /// <summary>Creates instance from 64-bit integer value in microseconds.</summary>
        /// <param name="valueUsec">Value in microseconds.</param>
        public Microseconds64(long valueUsec)
            => ValueUsec = valueUsec;

        /// <summary>Creates instance from <see cref="TimeSpan"/> value.</summary>
        /// <param name="value">This value will be converted from <see cref="TimeSpan.Ticks"/> to microseconds.</param>
        public Microseconds64(TimeSpan value)
            => ValueUsec = value.Ticks / UsecToTimeSpanTicksFactor;

        /// <summary>Converts to <see cref="TimeSpan"/>.</summary>
        /// <returns><see cref="TimeSpan"/> representation of this value.</returns>
        public TimeSpan ToTimeSpan()
            => TimeSpan.FromTicks(ValueUsec * UsecToTimeSpanTicksFactor);

        /// <summary>The total number of seconds represented by this instance.</summary>
        public double TotalSeconds => ValueUsec / 1_000_000.0;

        /// <summary>The total number of milliseconds represented by this instance.</summary>
        public double TotalMilliseconds => ValueUsec / 1_000.0;

        /// <summary>Equality exactly like <see cref="Int64"/> type has.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(Microseconds64 other)
            => ValueUsec.Equals(other.ValueUsec);

        /// <summary>Equality with another value specified as <see cref="TimeSpan"/>.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(TimeSpan other)
            => Equals(new Microseconds64(other));

        /// <summary>Equality exactly like <see cref="Int64"/> type has.</summary>
        /// <param name="otherUsec">Another value in microseconds to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(long otherUsec)
            => ValueUsec.Equals(otherUsec);

        /// <summary>Two values comparison exactly like <see cref="Int64"/> type has.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int64.CompareTo(long)"/>.
        /// </returns>
        public int CompareTo(Microseconds64 other)
            => ValueUsec.CompareTo(other.ValueUsec);

        /// <summary>Two values comparison.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int64.CompareTo(long)"/>.
        /// </returns>
        public int CompareTo(TimeSpan other)
            => CompareTo(new Microseconds64(other));

        /// <summary>Two values comparison exactly like <see cref="Int64"/> type has.</summary>
        /// <param name="otherUsec">Another value in microseconds to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int64.CompareTo(long)"/>.
        /// </returns>
        public int CompareTo(long otherUsec)
            => ValueUsec.CompareTo(otherUsec);

        /// <summary>Can compare current instance with <see cref="Microseconds64"/>, <see cref="TimeSpan"/> and <see cref="IConvertible"/> value.</summary>
        /// <param name="obj">Value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int64.CompareTo(long)"/>.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is not comparable with this one.</exception>
        public int CompareTo(object? obj)
            => obj switch
            {
                null => 1,
                Microseconds64 microseconds => CompareTo(microseconds),
                TimeSpan span => CompareTo(span),
                IConvertible _ => CompareTo(Convert.ToInt64(obj)),
                _ => throw new ArgumentException("Object is not a Microseconds64 or TimeSpan or 64-bit integer number", nameof(obj))
            };

        /// <summary>String representation of current instance.</summary>
        /// <param name="format">The format to use or <see langword="null"/> for default format.</param>
        /// <param name="formatProvider">The provider to use to format the value or <see langword="null"/> to obtain the numeric format information from the current locale setting.</param>
        /// <returns><c>{value} usec</c></returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => ValueUsec.ToString(format, formatProvider) + UNIT_POSTFIX;

        /// <summary>Overloads <see cref="Object.Equals(object)"/> to be consistent with <see cref="Equals(Microseconds64)"/>.</summary>
        /// <param name="obj">Object to be compared with this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> can be cast to <see cref="Microseconds64"/> and result is equal to this one.</returns>
        public override bool Equals(object? obj)
            => obj switch
            {
                null => false,
                Microseconds64 microseconds => Equals(microseconds),
                TimeSpan span => Equals(span),
                IConvertible _ => Equals(Convert.ToInt64(obj)),
                _ => false
            };

        /// <summary>Calculates hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        public override int GetHashCode()
            => ValueUsec.GetHashCode();

        /// <summary>String representation of current instance.</summary>
        /// <returns><c>{value} usec</c></returns>
        public override string ToString()
            => ValueUsec.ToString() + UNIT_POSTFIX;

        /// <summary>To be consistent with <see cref="Equals(Microseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Microseconds64)"/>
        public static bool operator ==(Microseconds64 left, Microseconds64 right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Microseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Microseconds64)"/>
        public static bool operator !=(Microseconds64 left, Microseconds64 right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="CompareTo(Microseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Microseconds64)"/>
        public static bool operator <(Microseconds64 left, Microseconds64 right)
            => left.CompareTo(right) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(Microseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Microseconds64)"/>
        public static bool operator >(Microseconds64 left, Microseconds64 right)
            => left.CompareTo(right) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(Microseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Microseconds64)"/>
        public static bool operator <=(Microseconds64 left, Microseconds64 right)
            => left.CompareTo(right) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(Microseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Microseconds64)"/>
        public static bool operator >=(Microseconds64 left, Microseconds64 right)
            => left.CompareTo(right) >= 0;

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Microseconds64)"/>
        public static bool operator ==(Microseconds64 left, TimeSpan right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Microseconds64)"/>
        public static bool operator !=(Microseconds64 left, TimeSpan right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <(Microseconds64 left, TimeSpan right)
            => left.CompareTo(right) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >(Microseconds64 left, TimeSpan right)
            => left.CompareTo(right) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <=(Microseconds64 left, TimeSpan right)
            => left.CompareTo(right) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >=(Microseconds64 left, TimeSpan right)
            => left.CompareTo(right) >= 0;

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator ==(TimeSpan left, Microseconds64 right)
            => right.Equals(left);

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator !=(TimeSpan left, Microseconds64 right)
            => !right.Equals(left);

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <(TimeSpan left, Microseconds64 right)
            => right.CompareTo(left) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >(TimeSpan left, Microseconds64 right)
            => right.CompareTo(left) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <=(TimeSpan left, Microseconds64 right)
            => right.CompareTo(left) >= 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >=(TimeSpan left, Microseconds64 right)
            => right.CompareTo(left) <= 0;

        /// <summary>To be consistent with <see cref="Equals(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="Equals(long)"/>
        public static bool operator ==(Microseconds64 left, long rightUsec)
            => left.Equals(rightUsec);

        /// <summary>To be consistent with <see cref="Equals(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="Equals(long)"/>
        public static bool operator !=(Microseconds64 left, long rightUsec)
            => !left.Equals(rightUsec);

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator <(Microseconds64 left, long rightUsec)
            => left.CompareTo(rightUsec) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator >(Microseconds64 left, long rightUsec)
            => left.CompareTo(rightUsec) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator <=(Microseconds64 left, long rightUsec)
            => left.CompareTo(rightUsec) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator >=(Microseconds64 left, long rightUsec)
            => left.CompareTo(rightUsec) >= 0;

        /// <summary>To be consistent with <see cref="Equals(long)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(long)"/>
        public static bool operator ==(long leftUsec, Microseconds64 right)
            => right.Equals(leftUsec);

        /// <summary>To be consistent with <see cref="Equals(long)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(long)"/>
        public static bool operator !=(long leftUsec, Microseconds64 right)
            => !right.Equals(leftUsec);

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator <(long leftUsec, Microseconds64 right)
            => right.CompareTo(leftUsec) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator >(long leftUsec, Microseconds64 right)
            => right.CompareTo(leftUsec) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator <=(long leftUsec, Microseconds64 right)
            => right.CompareTo(leftUsec) >= 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator >=(long leftUsec, Microseconds64 right)
            => right.CompareTo(leftUsec) <= 0;

        /// <summary>Implicit conversion to <see cref="TimeSpan"/>.</summary>
        /// <param name="value">Value to be converted to <see cref="TimeSpan"/>.</param>
        public static implicit operator TimeSpan(Microseconds64 value)
            => value.ToTimeSpan();

        /// <summary>Implicit conversion from <see cref="TimeSpan"/>.</summary>
        /// <param name="value">Value to be converted to <see cref="Microseconds64"/>.</param>
        public static implicit operator Microseconds64(TimeSpan value)
            => new(value);

        /// <summary>Implicit conversion to <see cref="long"/> value in microseconds.</summary>
        /// <param name="value">Value to be converted to <see cref="long"/>.</param>
        public static implicit operator long(Microseconds64 value)
            => value.ValueUsec;

        /// <summary>Implicit conversion from <see cref="long"/>.</summary>
        /// <param name="valueUsec">Value in microseconds to be converted to <see cref="Microseconds64"/>.</param>
        public static implicit operator Microseconds64(long valueUsec)
            => new(valueUsec);

        /// <summary>Creates instance of <see cref="Microseconds64"/> from seconds.</summary>
        /// <param name="valueSec">Value in seconds.</param>
        /// <returns>Created value.</returns>
        public static Microseconds64 FromSeconds(double valueSec)
            => new((long)(valueSec * 1_000_000));

        /// <summary>Creates instance of <see cref="Microseconds64"/> from milliseconds.</summary>
        /// <param name="valueMs">Value in milliseconds.</param>
        /// <returns>Created value.</returns>
        public static Microseconds64 FromMilliseconds(double valueMs)
            => new((long)(valueMs * 1_000));

        /// <summary>Zero value.</summary>
        public static readonly Microseconds64 Zero = new(0);

        internal static readonly long UsecToTimeSpanTicksFactor = TimeSpan.TicksPerSecond / 1_000_000L;
        internal const string UNIT_POSTFIX = " usec";
    }
}

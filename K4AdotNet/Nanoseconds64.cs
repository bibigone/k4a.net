using System;

namespace K4AdotNet
{
    /// <summary>64-bit time value in nanoseconds. Used for system timestamps.</summary>
    /// <remarks>
    /// Actually, this structure is an simple wrapper of <see cref="Int64"/> type.
    /// And <see cref="Nanoseconds64"/> value can be smoothly converted to/from <see cref="long"/> and <see cref="TimeSpan"/> values for convenience of usage in your code.
    /// </remarks>
    public struct Nanoseconds64 :
        IEquatable<Nanoseconds64>, IEquatable<TimeSpan>, IEquatable<long>,
        IComparable<Nanoseconds64>, IComparable<TimeSpan>, IComparable<long>, IComparable,
        IFormattable
    {
        /// <summary>Value in nanoseconds.</summary>
        /// <remarks>This structure is an wrapper around this value.</remarks>
        public long ValueNsec;

        /// <summary>Creates instance from 64-bit integer value in nanoseconds.</summary>
        /// <param name="valueNsec">Value in nanoseconds.</param>
        public Nanoseconds64(long valueNsec)
            => ValueNsec = valueNsec;

        /// <summary>Creates instance from <see cref="TimeSpan"/> value.</summary>
        /// <param name="value">This value will be converted from <see cref="TimeSpan.Ticks"/> to nanoseconds.</param>
        public Nanoseconds64(TimeSpan value)
            => ValueNsec = value.Ticks * NsecToTimeSpanTicksFactor;

        /// <summary>Converts to <see cref="TimeSpan"/>.</summary>
        /// <returns><see cref="TimeSpan"/> representation of this value.</returns>
        public TimeSpan ToTimeSpan()
            => TimeSpan.FromTicks(ValueNsec / NsecToTimeSpanTicksFactor);

        /// <summary>The total number of seconds represented by this instance.</summary>
        public double TotalSeconds => ValueNsec / 1_000_000_000.0;

        /// <summary>The total number of milliseconds represented by this instance.</summary>
        public double TotalMilliseconds => ValueNsec / 1_000_000.0;

        /// <summary>The total number of microseconds represented by this instance.</summary>
        public double TotalMicroseconds => ValueNsec / 1_000.0;

        /// <summary>Equality exactly like <see cref="Int64"/> type has.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(Nanoseconds64 other)
            => ValueNsec.Equals(other.ValueNsec);

        /// <summary>Equality with another value specified as <see cref="TimeSpan"/>.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(TimeSpan other)
            => Equals(new Nanoseconds64(other));

        /// <summary>Equality exactly like <see cref="Int64"/> type has.</summary>
        /// <param name="otherNsec">Another value in nanoseconds to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(long otherNsec)
            => ValueNsec.Equals(otherNsec);

        /// <summary>Two values comparison exactly like <see cref="Int64"/> type has.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int64.CompareTo(long)"/>.
        /// </returns>
        public int CompareTo(Nanoseconds64 other)
            => ValueNsec.CompareTo(other.ValueNsec);

        /// <summary>Two values comparison.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int64.CompareTo(long)"/>.
        /// </returns>
        public int CompareTo(TimeSpan other)
            => CompareTo(new Nanoseconds64(other));

        /// <summary>Two values comparison exactly like <see cref="Int64"/> type has.</summary>
        /// <param name="otherNsec">Another value in nanoseconds to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int64.CompareTo(long)"/>.
        /// </returns>
        public int CompareTo(long otherNsec)
            => ValueNsec.CompareTo(otherNsec);

        /// <summary>Can compare current instance with <see cref="Nanoseconds64"/>, <see cref="TimeSpan"/> and <see cref="IConvertible"/> value.</summary>
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
                Nanoseconds64 nanoseconds => CompareTo(nanoseconds),
                TimeSpan span => CompareTo(span),
                IConvertible _ => CompareTo(Convert.ToInt64(obj)),
                _ => throw new ArgumentException("Object is not a Nanoseconds64 or TimeSpan or 64-bit integer number", nameof(obj))
            };

        /// <summary>String representation of current instance.</summary>
        /// <param name="format">The format to use or <see langword="null"/> for default format.</param>
        /// <param name="formatProvider">The provider to use to format the value or <see langword="null"/> to obtain the numeric format information from the current locale setting.</param>
        /// <returns><c>{value} nsec</c></returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => ValueNsec.ToString(format, formatProvider) + UNIT_POSTFIX;

        /// <summary>Overloads <see cref="Object.Equals(object)"/> to be consistent with <see cref="Equals(Nanoseconds64)"/>.</summary>
        /// <param name="obj">Object to be compared with this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> can be cast to <see cref="Nanoseconds64"/> and result is equal to this one.</returns>
        public override bool Equals(object? obj)
            => obj switch
            {
                null => false,
                Nanoseconds64 nanoseconds => Equals(nanoseconds),
                TimeSpan span => Equals(span),
                IConvertible _ => Equals(Convert.ToInt64(obj)),
                _ => false
            };

        /// <summary>Calculates hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        public override int GetHashCode()
            => ValueNsec.GetHashCode();

        /// <summary>String representation of current instance.</summary>
        /// <returns><c>{value} nsec</c></returns>
        public override string ToString()
            => ValueNsec.ToString() + UNIT_POSTFIX;

        /// <summary>To be consistent with <see cref="Equals(Nanoseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Nanoseconds64)"/>
        public static bool operator ==(Nanoseconds64 left, Nanoseconds64 right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Nanoseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Nanoseconds64)"/>
        public static bool operator !=(Nanoseconds64 left, Nanoseconds64 right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="CompareTo(Nanoseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Nanoseconds64)"/>
        public static bool operator <(Nanoseconds64 left, Nanoseconds64 right)
            => left.CompareTo(right) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(Nanoseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Nanoseconds64)"/>
        public static bool operator >(Nanoseconds64 left, Nanoseconds64 right)
            => left.CompareTo(right) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(Nanoseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Nanoseconds64)"/>
        public static bool operator <=(Nanoseconds64 left, Nanoseconds64 right)
            => left.CompareTo(right) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(Nanoseconds64)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Nanoseconds64)"/>
        public static bool operator >=(Nanoseconds64 left, Nanoseconds64 right)
            => left.CompareTo(right) >= 0;

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Nanoseconds64)"/>
        public static bool operator ==(Nanoseconds64 left, TimeSpan right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Nanoseconds64)"/>
        public static bool operator !=(Nanoseconds64 left, TimeSpan right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <(Nanoseconds64 left, TimeSpan right)
            => left.CompareTo(right) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >(Nanoseconds64 left, TimeSpan right)
            => left.CompareTo(right) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <=(Nanoseconds64 left, TimeSpan right)
            => left.CompareTo(right) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >=(Nanoseconds64 left, TimeSpan right)
            => left.CompareTo(right) >= 0;

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator ==(TimeSpan left, Nanoseconds64 right)
            => right.Equals(left);

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator !=(TimeSpan left, Nanoseconds64 right)
            => !right.Equals(left);

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <(TimeSpan left, Nanoseconds64 right)
            => right.CompareTo(left) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >(TimeSpan left, Nanoseconds64 right)
            => right.CompareTo(left) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <=(TimeSpan left, Nanoseconds64 right)
            => right.CompareTo(left) >= 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >=(TimeSpan left, Nanoseconds64 right)
            => right.CompareTo(left) <= 0;

        /// <summary>To be consistent with <see cref="Equals(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightNsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="rightNsec"/>.</returns>
        /// <seealso cref="Equals(long)"/>
        public static bool operator ==(Nanoseconds64 left, long rightNsec)
            => left.Equals(rightNsec);

        /// <summary>To be consistent with <see cref="Equals(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightNsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="rightNsec"/>.</returns>
        /// <seealso cref="Equals(long)"/>
        public static bool operator !=(Nanoseconds64 left, long rightNsec)
            => !left.Equals(rightNsec);

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightNsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="rightNsec"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator <(Nanoseconds64 left, long rightNsec)
            => left.CompareTo(rightNsec) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightNsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="rightNsec"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator >(Nanoseconds64 left, long rightNsec)
            => left.CompareTo(rightNsec) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightNsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="rightNsec"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator <=(Nanoseconds64 left, long rightNsec)
            => left.CompareTo(rightNsec) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightNsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="rightNsec"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator >=(Nanoseconds64 left, long rightNsec)
            => left.CompareTo(rightNsec) >= 0;

        /// <summary>To be consistent with <see cref="Equals(long)"/>.</summary>
        /// <param name="leftNsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftNsec"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(long)"/>
        public static bool operator ==(long leftNsec, Nanoseconds64 right)
            => right.Equals(leftNsec);

        /// <summary>To be consistent with <see cref="Equals(long)"/>.</summary>
        /// <param name="leftNsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftNsec"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(long)"/>
        public static bool operator !=(long leftNsec, Nanoseconds64 right)
            => !right.Equals(leftNsec);

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="leftNsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftNsec"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator <(long leftNsec, Nanoseconds64 right)
            => right.CompareTo(leftNsec) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="leftNsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftNsec"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator >(long leftNsec, Nanoseconds64 right)
            => right.CompareTo(leftNsec) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="leftNsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftNsec"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator <=(long leftNsec, Nanoseconds64 right)
            => right.CompareTo(leftNsec) >= 0;

        /// <summary>To be consistent with <see cref="CompareTo(long)"/>.</summary>
        /// <param name="leftNsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftNsec"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(long)"/>
        public static bool operator >=(long leftNsec, Nanoseconds64 right)
            => right.CompareTo(leftNsec) <= 0;

        /// <summary>Sum of two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator +(Nanoseconds64 left, Nanoseconds64 right)
            => left.ValueNsec + right.ValueNsec;

        /// <summary>Sum of two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator +(Nanoseconds64 left, Microseconds64 right)
            => left.ValueNsec + right.ValueUsec * 1_000L;

        /// <summary>Sum of two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator +(Microseconds64 left, Nanoseconds64 right)
            => left.ValueUsec * 1_000L + right.ValueNsec;

        /// <summary>Sum of two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator +(Nanoseconds64 left, Microseconds32 right)
            => left.ValueNsec + right.ValueUsec * 1_000L;

        /// <summary>Sum of two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator +(Microseconds32 left, Nanoseconds64 right)
            => left.ValueUsec * 1_000L + right.ValueNsec;

        /// <summary>Sum of two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator +(Nanoseconds64 left, long right)
            => left.ValueNsec + right;

        /// <summary>Sum of two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator +(Nanoseconds64 left, int right)
            => left.ValueNsec + right;

        /// <summary>Difference between two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Difference between <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator -(Nanoseconds64 left, Nanoseconds64 right)
            => left.ValueNsec - right.ValueNsec;

        /// <summary>Difference between two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Difference between <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator -(Nanoseconds64 left, Microseconds64 right)
            => left.ValueNsec - right.ValueUsec * 1_000L;

        /// <summary>Difference between two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Difference between <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator -(Microseconds32 left, Nanoseconds64 right)
            => left.ValueUsec * 1_000L - right.ValueNsec;

        /// <summary>Difference between two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Difference between <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator -(Nanoseconds64 left, Microseconds32 right)
            => left.ValueNsec - right.ValueUsec * 1_000L;

        /// <summary>Difference between two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Difference between <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator -(Microseconds64 left, Nanoseconds64 right)
            => left.ValueUsec * 1_000L - right.ValueNsec;

        /// <summary>Difference between two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Difference between <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator -(Nanoseconds64 left, long right)
            => left.ValueNsec - right;

        /// <summary>Difference between two time stamps.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns>Difference between <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator -(Nanoseconds64 left, int right)
            => left.ValueNsec - right;

        /// <summary>Multiplies time stamp by an integer number.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns><paramref name="left"/> times <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator *(Nanoseconds64 left, int right)
            => left.ValueNsec * right;

        /// <summary>Multiplies time stamp by an integer number.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns><paramref name="right"/> times <paramref name="left"/>.</returns>
        public static Nanoseconds64 operator *(int left, Nanoseconds64 right)
            => left * right.ValueNsec;

        /// <summary>Divides time stamp by an integer number.</summary>
        /// <param name="left">Left value.</param>
        /// <param name="right">Right value.</param>
        /// <returns><paramref name="left"/> divided by <paramref name="right"/>.</returns>
        public static Nanoseconds64 operator /(Nanoseconds64 left, int right)
            => left.ValueNsec / right;

        /// <summary>Implicit conversion to <see cref="TimeSpan"/>.</summary>
        /// <param name="value">Value to be converted to <see cref="TimeSpan"/>.</param>
        public static implicit operator TimeSpan(Nanoseconds64 value)
            => value.ToTimeSpan();

        /// <summary>Implicit conversion from <see cref="TimeSpan"/>.</summary>
        /// <param name="value">Value to be converted to <see cref="Nanoseconds64"/>.</param>
        public static implicit operator Nanoseconds64(TimeSpan value)
            => new(value);

        /// <summary>Implicit conversion from <see cref="long"/>.</summary>
        /// <param name="valueNsec">Value in nanoseconds to be converted to <see cref="Nanoseconds64"/>.</param>
        public static implicit operator Nanoseconds64(long valueNsec)
            => new(valueNsec);

        /// <summary>Creates instance of <see cref="Nanoseconds64"/> from seconds.</summary>
        /// <param name="valueSec">Value in seconds.</param>
        /// <returns>Created value.</returns>
        public static Nanoseconds64 FromSeconds(double valueSec)
            => new((long)(valueSec * 1_000_000_000));

        /// <summary>Creates instance of <see cref="Nanoseconds64"/> from milliseconds.</summary>
        /// <param name="valueMs">Value in milliseconds.</param>
        /// <returns>Created value.</returns>
        public static Nanoseconds64 FromMilliseconds(double valueMs)
            => new((long)(valueMs * 1_000_000));

        /// <summary>Creates instance of <see cref="Nanoseconds64"/> from microseconds.</summary>
        /// <param name="valueUs">Value in microseconds.</param>
        /// <returns>Created value.</returns>
        public static Nanoseconds64 FromMicroseconds(double valueUs)
            => new((long)(valueUs * 1_000));

        /// <summary>Zero value.</summary>
        public static readonly Nanoseconds64 Zero = new(0);

        private static readonly long NsecToTimeSpanTicksFactor = 1_000_000_000L / TimeSpan.TicksPerSecond;
        private const string UNIT_POSTFIX = " nsec";
    }
}

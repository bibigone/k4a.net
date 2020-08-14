using System;

namespace K4AdotNet
{
    /// <summary>32-bit time value in microseconds. Used for timestamps and delays.</summary>
    /// <remarks>
    /// Actually, this structure is an simple wrapper of <see cref="Int32"/> type.
    /// And <see cref="Microseconds32"/> value can be smoothly converted to/from <see cref="int"/> and <see cref="TimeSpan"/> values for convenience of usage in your code.
    /// </remarks>
    public struct Microseconds32 :
        IEquatable<Microseconds32>, IEquatable<TimeSpan>, IEquatable<int>,
        IComparable<Microseconds32>, IComparable<TimeSpan>, IComparable<int>, IComparable,
        IFormattable
    {
        /// <summary>Value in microseconds.</summary>
        /// <remarks>This structure is an wrapper around this value.</remarks>
        public int ValueUsec;

        /// <summary>Creates instance from integer value in microseconds.</summary>
        /// <param name="valueUsec">Value in microseconds.</param>
        public Microseconds32(int valueUsec)
            => ValueUsec = valueUsec;

        /// <summary>Creates instance from <see cref="TimeSpan"/> value.</summary>
        /// <param name="value">This value will be converted from <see cref="TimeSpan.Ticks"/> to microseconds.</param>
        public Microseconds32(TimeSpan value)
            => ValueUsec = checked((int)(value.Ticks / Microseconds64.UsecToTimeSpanTicksFactor));

        /// <summary>Converts to <see cref="TimeSpan"/>.</summary>
        /// <returns><see cref="TimeSpan"/> representation of this value.</returns>
        public TimeSpan ToTimeSpan()
            => TimeSpan.FromTicks(ValueUsec * Microseconds64.UsecToTimeSpanTicksFactor);

        /// <summary>The total number of seconds represented by this instance.</summary>
        public double TotalSeconds => ValueUsec / 1_000_000.0;

        /// <summary>The total number of milliseconds represented by this instance.</summary>
        public double TotalMilliseconds => ValueUsec / 1_000.0;

        /// <summary>Equality exactly like <see cref="Int32"/> type has.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(Microseconds32 other)
            => ValueUsec.Equals(other.ValueUsec);

        /// <summary>Equality with another value specified as <see cref="TimeSpan"/>.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(TimeSpan other)
            => Equals(new Microseconds32(other));

        /// <summary>Equality exactly like <see cref="Int32"/> type has.</summary>
        /// <param name="otherUsec">Another value in microseconds to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(int otherUsec)
            => ValueUsec.Equals(otherUsec);

        /// <summary>Two values comparison exactly like <see cref="Int32"/> type has.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int32.CompareTo(int)"/>.
        /// </returns>
        public int CompareTo(Microseconds32 other)
            => ValueUsec.CompareTo(other.ValueUsec);

        /// <summary>Two values comparison.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int32.CompareTo(int)"/>.
        /// </returns>
        public int CompareTo(TimeSpan other)
            => CompareTo(new Microseconds32(other));

        /// <summary>Two values comparison exactly like <see cref="Int32"/> type has.</summary>
        /// <param name="otherUsec">Another value in microseconds to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int32.CompareTo(int)"/>.
        /// </returns>
        public int CompareTo(int otherUsec)
            => ValueUsec.CompareTo(otherUsec);

        /// <summary>Can compare current instance with <see cref="Microseconds32"/>, <see cref="TimeSpan"/> and <see cref="IConvertible"/> value.</summary>
        /// <param name="obj">Value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="Int32.CompareTo(int)"/>.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is not comparable with this one.</exception>
        public int CompareTo(object? obj)
            => obj switch
            {
                null => 1,
                Microseconds32 _ => CompareTo((Microseconds32)obj),
                TimeSpan _ => CompareTo((TimeSpan)obj),
                IConvertible _ => CompareTo(Convert.ToInt32(obj)),
                _ => throw new ArgumentException("Object is not a Microseconds32 or TimeSpan or integer number", nameof(obj))
            };

        /// <summary>String representation of current instance.</summary>
        /// <param name="format">The format to use or <see langword="null"/> for default format.</param>
        /// <param name="formatProvider">The provider to use to format the value or <see langword="null"/> to obtain the numeric format information from the current locale setting.</param>
        /// <returns><c>{value} usec</c></returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => ValueUsec.ToString(format, formatProvider) + Microseconds64.UNIT_POSTFIX;

        /// <summary>Overloads <see cref="Object.Equals(object)"/> to be consistent with <see cref="Equals(Microseconds32)"/>.</summary>
        /// <param name="obj">Object to be compared with this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> can be cast to <see cref="Microseconds32"/> and result is equal to this one.</returns>
        public override bool Equals(object? obj)
            => obj switch
            {
                null => false,
                Microseconds32 _ => Equals((Microseconds32)obj),
                TimeSpan _ => Equals((TimeSpan)obj),
                IConvertible _ => Equals(Convert.ToInt32(obj)),
                _ => false
            };

        /// <summary>Calculates hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        public override int GetHashCode()
            => ValueUsec.GetHashCode();

        /// <summary>String representation of current instance.</summary>
        /// <returns><c>{value} usec</c></returns>
        public override string ToString()
            => ValueUsec.ToString() + Microseconds64.UNIT_POSTFIX;

        /// <summary>To be consistent with <see cref="Equals(Microseconds32)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Microseconds32)"/>
        public static bool operator ==(Microseconds32 left, Microseconds32 right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Microseconds32)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Microseconds32)"/>
        public static bool operator !=(Microseconds32 left, Microseconds32 right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="CompareTo(Microseconds32)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Microseconds32)"/>
        public static bool operator <(Microseconds32 left, Microseconds32 right)
            => left.CompareTo(right) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(Microseconds32)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Microseconds32)"/>
        public static bool operator >(Microseconds32 left, Microseconds32 right)
            => left.CompareTo(right) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(Microseconds32)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Microseconds32)"/>
        public static bool operator <=(Microseconds32 left, Microseconds32 right)
            => left.CompareTo(right) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(Microseconds32)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Microseconds32)"/>
        public static bool operator >=(Microseconds32 left, Microseconds32 right)
            => left.CompareTo(right) >= 0;

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator ==(Microseconds32 left, TimeSpan right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator !=(Microseconds32 left, TimeSpan right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <(Microseconds32 left, TimeSpan right)
            => left.CompareTo(right) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >(Microseconds32 left, TimeSpan right)
            => left.CompareTo(right) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <=(Microseconds32 left, TimeSpan right)
            => left.CompareTo(right) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >=(Microseconds32 left, TimeSpan right)
            => left.CompareTo(right) >= 0;

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator ==(TimeSpan left, Microseconds32 right)
            => right.Equals(left);

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator !=(TimeSpan left, Microseconds32 right)
            => !right.Equals(left);

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <(TimeSpan left, Microseconds32 right)
            => right.CompareTo(left) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >(TimeSpan left, Microseconds32 right)
            => right.CompareTo(left) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <=(TimeSpan left, Microseconds32 right)
            => right.CompareTo(left) >= 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >=(TimeSpan left, Microseconds32 right)
            => right.CompareTo(left) <= 0;

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator ==(Microseconds32 left, int rightUsec)
            => left.Equals(rightUsec);

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator !=(Microseconds32 left, int rightUsec)
            => !left.Equals(rightUsec);

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator <(Microseconds32 left, int rightUsec)
            => left.CompareTo(rightUsec) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator >(Microseconds32 left, int rightUsec)
            => left.CompareTo(rightUsec) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator <=(Microseconds32 left, int rightUsec)
            => left.CompareTo(rightUsec) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightUsec">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="rightUsec"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator >=(Microseconds32 left, int rightUsec)
            => left.CompareTo(rightUsec) >= 0;

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator ==(int leftUsec, Microseconds32 right)
            => right.Equals(leftUsec);

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator !=(int leftUsec, Microseconds32 right)
            => !right.Equals(leftUsec);

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator <(int leftUsec, Microseconds32 right)
            => right.CompareTo(leftUsec) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator >(int leftUsec, Microseconds32 right)
            => right.CompareTo(leftUsec) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator <=(int leftUsec, Microseconds32 right)
            => right.CompareTo(leftUsec) >= 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="leftUsec">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftUsec"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator >=(int leftUsec, Microseconds32 right)
            => right.CompareTo(leftUsec) <= 0;

        /// <summary>Implicit conversion to <see cref="TimeSpan"/>.</summary>
        /// <param name="value">Value to be converted to <see cref="TimeSpan"/>.</param>
        /// <seealso cref="ToTimeSpan"/>
        public static implicit operator TimeSpan(Microseconds32 value)
            => value.ToTimeSpan();

        /// <summary>Implicit conversion from <see cref="TimeSpan"/>.</summary>
        /// <param name="value">Value to be converted to <see cref="Microseconds32"/>.</param>
        public static implicit operator Microseconds32(TimeSpan value)
            => new Microseconds32(value);

        /// <summary>Implicit conversion to <see cref="int"/> value in microseconds.</summary>
        /// <param name="value">Value to be converted to <see cref="int"/>.</param>
        public static implicit operator int(Microseconds32 value)
            => value.ValueUsec;

        /// <summary>Implicit conversion from <see cref="int"/>.</summary>
        /// <param name="valueUsec">Value in microseconds to be converted to <see cref="Microseconds32"/>.</param>
        public static implicit operator Microseconds32(int valueUsec)
            => new Microseconds32(valueUsec);

        /// <summary>Creates instance of <see cref="Microseconds32"/> from seconds.</summary>
        /// <param name="valueSec">Value in seconds.</param>
        /// <returns>Created value.</returns>
        public static Microseconds32 FromSeconds(double valueSec)
            => new Microseconds32((int)(valueSec * 1_000_000));

        /// <summary>Creates instance of <see cref="Microseconds32"/> from milliseconds.</summary>
        /// <param name="valueMs">Value in milliseconds.</param>
        /// <returns>Created value.</returns>
        public static Microseconds32 FromMilliseconds(double valueMs)
            => new Microseconds32((int)(valueMs * 1_000));

        /// <summary>Zero value.</summary>
        public static readonly Microseconds32 Zero = new Microseconds32(0);
    }
}

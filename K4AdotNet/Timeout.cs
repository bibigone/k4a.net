using System;
using System.Runtime.InteropServices;

namespace K4AdotNet
{
    /// <summary>32-bit timeout value in milliseconds. Used in API to define timeouts to wait.</summary>
    /// <remarks>
    /// Actually, this structure is an simple wrapper of <see cref="Int32"/> type.
    /// And <see cref="Timeout"/> value can be smoothly converted to/from <see cref="int"/> and <see cref="TimeSpan"/> values for convenience of usage in your code.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct Timeout :
        IEquatable<Timeout>, IEquatable<TimeSpan>, IEquatable<int>,
        IComparable<Timeout>, IComparable<TimeSpan>, IComparable<int>, IComparable,
        IFormattable
    {
        /// <summary>Value in milliseconds. Should be of <see cref="uint"/> type but for CLS-compatibility it is declared is <see cref="int"/>.</summary>
        /// <remarks><para>
        /// There are two special values:
        /// 0 - means "no wait" (see <see cref="NoWait"/>),
        /// -1 (that is <see cref="uint.MaxValue"/>) - means infinite waiting (see <see cref="Infinite"/>).
        /// </para><para>
        /// This structure is an wrapper around this value.
        /// </para></remarks>
        public int ValueMs;

        /// <summary>Creates instance from integer value in milliseconds.</summary>
        /// <param name="valueMs">Timeout in milliseconds, or -1 for infinite timeout (see <see cref="Infinite"/>).</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="valueMs"/> is less than -1.</exception>
        public Timeout(int valueMs)
        {
            if (valueMs < -1)
                throw new ArgumentOutOfRangeException(nameof(valueMs));
            ValueMs = valueMs;
        }

        /// <summary>Creates instance from <see cref="TimeSpan"/> value.</summary>
        /// <param name="value">Timeout specified as <see cref="TimeSpan"/> value. Use <see cref="TimeSpan.MaxValue"/> to specify infinite timeout (see <see cref="Infinite"/>).</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="value"/> is negative.</exception>
        public Timeout(TimeSpan value)
        {
            if (value.Ticks < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            ValueMs = value == TimeSpan.MaxValue
                ? Infinite.ValueMs
                : checked((int)value.TotalMilliseconds);
        }

        /// <summary>Converts timeout to <see cref="TimeSpan"/> value.</summary>
        /// <returns>Corresponding <see cref="TimeSpan"/> value. For infinite timeout (see <see cref="Infinite"/>) this method returns <see cref="TimeSpan.MaxValue"/>.</returns>
        public TimeSpan ToTimeSpan()
            => ValueMs == Infinite.ValueMs
                ? TimeSpan.MaxValue
                : TimeSpan.FromMilliseconds(unchecked((uint)ValueMs));

        /// <summary>The total number of seconds represented by this instance.</summary>
        public double TotalSeconds => ValueMs == Infinite.ValueMs ? double.MaxValue : unchecked((uint)ValueMs) / 1_000.0;

        /// <summary>The total number of milliseconds represented by this instance.</summary>
        public int TotalMilliseconds => ValueMs;

        /// <summary>Equality exactly like <see cref="int"/> type has.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(Timeout other)
            => ValueMs.Equals(other.ValueMs);

        /// <summary>Equality with another value specified as <see cref="TimeSpan"/>.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(TimeSpan other)
            => other.Ticks >= 0 && Equals(new Timeout(other));

        /// <summary>Equality exactly like <see cref="int"/> type has.</summary>
        /// <param name="otherMs">Another value in milliseconds to be compared with this one.</param>
        /// <returns><see langword="true"/> if values are equal.</returns>
        public bool Equals(int otherMs)
            => ValueMs.Equals(otherMs);

        /// <summary>Two values comparison exactly like <see cref="uint"/> type has.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="uint.CompareTo(uint)"/>.
        /// </returns>
        public int CompareTo(Timeout other)
            => unchecked((uint)ValueMs).CompareTo(unchecked((uint)other.ValueMs));

        /// <summary>Two values comparison.</summary>
        /// <param name="other">Another value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="uint.CompareTo(uint)"/>.
        /// </returns>
        public int CompareTo(TimeSpan other)
            => other.Ticks < 0 ? 1 : CompareTo(new Timeout(other));

        /// <summary>Two values comparison exactly like <see cref="uint"/> type has.</summary>
        /// <param name="otherMs">Another value in milliseconds to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="uint.CompareTo(uint)"/>.
        /// </returns>
        public int CompareTo(int otherMs)
            => otherMs < -1 ? 1 : CompareTo(new Timeout(otherMs));

        /// <summary>Can compare current instance with <see cref="Timeout"/>, <see cref="TimeSpan"/> and <see cref="IConvertible"/> value.</summary>
        /// <param name="obj">Value to be compared with this one.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value.
        /// For details see <see cref="uint.CompareTo(uint)"/>.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is not comparable with this one.</exception>
        public int CompareTo(object? obj)
            => obj switch
            {
                null => 1,
                Timeout timeout => CompareTo(timeout),
                TimeSpan span => CompareTo(span),
                IConvertible _ => CompareTo(Convert.ToInt32(obj)),
                _ => throw new ArgumentException("Object is not a Timeout or TimeSpan or integer number", nameof(obj))
            };

        /// <summary>String representation of current instance.</summary>
        /// <param name="format">The format to use or <see langword="null"/> for default format.</param>
        /// <param name="formatProvider">The provider to use to format the value or <see langword="null"/> to obtain the numeric format information from the current locale setting.</param>
        /// <returns><c>{value} ms</c> or <c>Infinite</c> string or <c>NoWait</c> string.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (ValueMs < 0)
                return nameof(Infinite);
            if (ValueMs == 0)
                return nameof(NoWait);
            return ValueMs.ToString(format, formatProvider) + UNIT_POSTFIX;
        }

        /// <summary>Overloads <see cref="Object.Equals(object)"/> to be consistent with <see cref="Equals(Timeout)"/>.</summary>
        /// <param name="obj">Object to be compared with this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> can be cast to <see cref="Microseconds32"/> and result is equal to this one.</returns>
        public override bool Equals(object? obj)
            => obj switch
            {
                null => false,
                Timeout timeout => Equals(timeout),
                TimeSpan span => Equals(span),
                IConvertible _ => Equals(Convert.ToInt32(obj)),
                _ => false
            };

        /// <summary>Calculates hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        public override int GetHashCode()
            => ValueMs;

        /// <summary>String representation of current instance.</summary>
        /// <returns><c>{value} ms</c> or <c>Infinite</c> string or <c>NoWait</c> string.</returns>
        public override string ToString()
        {
            if (ValueMs == Infinite.ValueMs)
                return nameof(Infinite);
            if (ValueMs == 0)
                return nameof(NoWait);
            return unchecked((uint)ValueMs).ToString() + UNIT_POSTFIX;
        }

        /// <summary>To be consistent with <see cref="Equals(Timeout)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Timeout)"/>
        public static bool operator ==(Timeout left, Timeout right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Timeout)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Timeout)"/>
        public static bool operator !=(Timeout left, Timeout right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="CompareTo(Timeout)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Timeout)"/>
        public static bool operator <(Timeout left, Timeout right)
            => left.CompareTo(right) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(Timeout)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Timeout)"/>
        public static bool operator <=(Timeout left, Timeout right)
            => left.CompareTo(right) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(Timeout)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Timeout)"/>
        public static bool operator >(Timeout left, Timeout right)
            => left.CompareTo(right) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(Timeout)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(Timeout)"/>
        public static bool operator >=(Timeout left, Timeout right)
            => left.CompareTo(right) >= 0;

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator ==(Timeout left, TimeSpan right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator !=(Timeout left, TimeSpan right)
            => !left.Equals(right);

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <(Timeout left, TimeSpan right)
            => left.CompareTo(right) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <=(Timeout left, TimeSpan right)
            => left.CompareTo(right) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >(Timeout left, TimeSpan right)
            => left.CompareTo(right) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >=(Timeout left, TimeSpan right)
            => left.CompareTo(right) >= 0;

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator ==(TimeSpan left, Timeout right)
            => right.Equals(left);

        /// <summary>To be consistent with <see cref="Equals(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(TimeSpan)"/>
        public static bool operator !=(TimeSpan left, Timeout right)
            => !right.Equals(left);

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <(TimeSpan left, Timeout right)
            => right.CompareTo(left) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator <=(TimeSpan left, Timeout right)
            => right.CompareTo(left) >= 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >(TimeSpan left, Timeout right)
            => right.CompareTo(left) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(TimeSpan)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than on equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(TimeSpan)"/>
        public static bool operator >=(TimeSpan left, Timeout right)
            => right.CompareTo(left) <= 0;

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightMs">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="rightMs"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator ==(Timeout left, int rightMs)
            => left.Equals(rightMs);

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightMs">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="rightMs"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator !=(Timeout left, int rightMs)
            => !left.Equals(rightMs);

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightMs">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than <paramref name="rightMs"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator <(Timeout left, int rightMs)
            => left.CompareTo(rightMs) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightMs">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is less than or equal to <paramref name="rightMs"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator <=(Timeout left, int rightMs)
            => left.CompareTo(rightMs) <= 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightMs">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than <paramref name="rightMs"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator >(Timeout left, int rightMs)
            => left.CompareTo(rightMs) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="rightMs">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is greater than or equal to <paramref name="rightMs"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator >=(Timeout left, int rightMs)
            => left.CompareTo(rightMs) >= 0;

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="leftMs">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftMs"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator ==(int leftMs, Timeout right)
            => right.Equals(leftMs);

        /// <summary>To be consistent with <see cref="Equals(int)"/>.</summary>
        /// <param name="leftMs">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftMs"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(int)"/>
        public static bool operator !=(int leftMs, Timeout right)
            => !right.Equals(leftMs);

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="leftMs">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftMs"/> is less than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator <(int leftMs, Timeout right)
            => right.CompareTo(leftMs) > 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="leftMs">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftMs"/> is less than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator <=(int leftMs, Timeout right)
            => right.CompareTo(leftMs) >= 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="leftMs">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftMs"/> is greater than <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator >(int leftMs, Timeout right)
            => right.CompareTo(leftMs) < 0;

        /// <summary>To be consistent with <see cref="CompareTo(int)"/>.</summary>
        /// <param name="leftMs">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="leftMs"/> is greater than or equal to <paramref name="right"/>.</returns>
        /// <seealso cref="CompareTo(int)"/>
        public static bool operator >=(int leftMs, Timeout right)
            => right.CompareTo(leftMs) <= 0;

        /// <summary>Implicit conversion to <see cref="TimeSpan"/>.</summary>
        /// <param name="value">Value to be converted to <see cref="TimeSpan"/>.</param>
        /// <seealso cref="ToTimeSpan"/>
        public static implicit operator TimeSpan(Timeout value)
            => value.ToTimeSpan();

        /// <summary>Implicit conversion from <see cref="TimeSpan"/>.</summary>
        /// <param name="value">Value to be converted to <see cref="Microseconds32"/>.</param>
        public static implicit operator Timeout(TimeSpan value)
            => new Timeout(value);

        /// <summary>Implicit conversion from <see cref="int"/>.</summary>
        /// <param name="valueMs">Value in milliseconds to be converted to <see cref="Timeout"/>.</param>
        public static implicit operator Timeout(int valueMs)
            => new Timeout(valueMs);

        /// <summary>Creates instance of <see cref="Timeout"/> from seconds.</summary>
        /// <param name="timeoutSec">Value in seconds.</param>
        /// <returns>Created value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="timeoutSec"/> is less than zero.</exception>
        public static Timeout FromSeconds(double timeoutSec)
        {
            if (timeoutSec < 0)
                throw new ArgumentOutOfRangeException(nameof(timeoutSec));
            return new Timeout((int)(timeoutSec * 1_000));
        }

        /// <summary>Creates instance of <see cref="Timeout"/> from milliseconds.</summary>
        /// <param name="timeoutMs">Value in milliseconds.</param>
        /// <returns>Created value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="timeoutMs"/> is less than zero.</exception>
        public static Timeout FromMilliseconds(int timeoutMs)
        {
            if (timeoutMs < 0)
                throw new ArgumentOutOfRangeException(nameof(timeoutMs));
            return new Timeout(timeoutMs);
        }

        /// <summary>Special timeout value: non blocking call.</summary>
        public static readonly Timeout NoWait = new Timeout(0);

        /// <summary>Special timeout value: infinite waiting.</summary>
        public static readonly Timeout Infinite = new Timeout(-1);

        private const string UNIT_POSTFIX = " ms";
    }
}

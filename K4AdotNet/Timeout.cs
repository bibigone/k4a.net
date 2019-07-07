using System;
using System.Runtime.InteropServices;

namespace K4AdotNet
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Timeout :
        IEquatable<Timeout>, IEquatable<TimeSpan>, IEquatable<int>,
        IComparable<Timeout>, IComparable<TimeSpan>, IComparable<int>, IComparable,
        IFormattable
    {
        public int ValueMs;

        public Timeout(int valueMs)
            => ValueMs = valueMs;

        public Timeout(TimeSpan value)
            => ValueMs = checked((int)value.TotalMilliseconds);

        public TimeSpan ToTimeSpan()
            => TimeSpan.FromMilliseconds(ValueMs);

        public bool Equals(Timeout other)
            => ValueMs.Equals(other.ValueMs);

        public bool Equals(TimeSpan other)
            => Equals(new Timeout(other));

        public bool Equals(int otherMs)
            => ValueMs.Equals(otherMs);

        public int CompareTo(Timeout other)
            => unchecked((uint)ValueMs).CompareTo(unchecked((uint)ValueMs));

        public int CompareTo(TimeSpan other)
            => CompareTo(new Timeout(other));

        public int CompareTo(int otherMs)
            => CompareTo(new Timeout(otherMs));

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;
            if (obj is Timeout)
                return CompareTo((Timeout)obj);
            if (obj is TimeSpan)
                return CompareTo((TimeSpan)obj);
            if (obj is IConvertible)
                return CompareTo(Convert.ToInt32(obj));
            throw new ArgumentException("Object is not a Timeout or TimeSpan or integer number", nameof(obj));
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (ValueMs < 0)
                return nameof(Infinite);
            if (ValueMs == 0)
                return nameof(NoWait);
            return ValueMs.ToString(format, formatProvider) + " ms";
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is Timeout)
                return Equals((Timeout)obj);
            if (obj is TimeSpan)
                return Equals((TimeSpan)obj);
            if (obj is IConvertible)
                return Equals(Convert.ToInt32(obj));
            return false;
        }

        public override int GetHashCode()
            => ValueMs;

        public override string ToString()
        {
            if (ValueMs < 0)
                return nameof(Infinite);
            if (ValueMs == 0)
                return nameof(NoWait);
            return ValueMs.ToString() + " ms";
        }

        public static bool operator ==(Timeout left, Timeout right)
            => left.Equals(right);

        public static bool operator !=(Timeout left, Timeout right)
            => !left.Equals(right);

        public static bool operator <(Timeout left, Timeout right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(Timeout left, Timeout right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(Timeout left, Timeout right)
            => left.CompareTo(right) < 0;

        public static bool operator >=(Timeout left, Timeout right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(Timeout left, TimeSpan right)
            => left.Equals(right);

        public static bool operator !=(Timeout left, TimeSpan right)
            => !left.Equals(right);

        public static bool operator <(Timeout left, TimeSpan right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(Timeout left, TimeSpan right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(Timeout left, TimeSpan right)
            => left.CompareTo(right) < 0;

        public static bool operator >=(Timeout left, TimeSpan right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(TimeSpan left, Timeout right)
            => new Timeout(left).Equals(right);

        public static bool operator !=(TimeSpan left, Timeout right)
            => !new Timeout(left).Equals(right);

        public static bool operator <(TimeSpan left, Timeout right)
            => new Timeout(left).CompareTo(right) < 0;

        public static bool operator <=(TimeSpan left, Timeout right)
            => new Timeout(left).CompareTo(right) <= 0;

        public static bool operator >(TimeSpan left, Timeout right)
            => new Timeout(left).CompareTo(right) < 0;

        public static bool operator >=(TimeSpan left, Timeout right)
            => new Timeout(left).CompareTo(right) >= 0;

        public static bool operator ==(Timeout left, int rightMs)
            => left.Equals(rightMs);

        public static bool operator !=(Timeout left, int rightMs)
            => !left.Equals(rightMs);

        public static bool operator <(Timeout left, int rightMs)
            => left.CompareTo(rightMs) < 0;

        public static bool operator <=(Timeout left, int rightMs)
            => left.CompareTo(rightMs) <= 0;

        public static bool operator >(Timeout left, int rightMs)
            => left.CompareTo(rightMs) < 0;

        public static bool operator >=(Timeout left, int rightMs)
            => left.CompareTo(rightMs) >= 0;

        public static bool operator ==(int leftMs, Timeout rightMs)
            => new Timeout(leftMs).Equals(rightMs);

        public static bool operator !=(int leftMs, Timeout rightMs)
            => !new Timeout(leftMs).Equals(rightMs);

        public static bool operator <(int leftMs, Timeout right)
            => new Timeout(leftMs).CompareTo(right) < 0;

        public static bool operator <=(int leftMs, Timeout right)
            => new Timeout(leftMs).CompareTo(right) <= 0;

        public static bool operator >(int leftMs, Timeout right)
            => new Timeout(leftMs).CompareTo(right) < 0;

        public static bool operator >=(int leftMs, Timeout right)
            => new Timeout(leftMs).CompareTo(right) >= 0;

        public static implicit operator TimeSpan(Timeout value)
            => value.ToTimeSpan();

        public static implicit operator Timeout(TimeSpan value)
            => new Timeout(value);

        public static implicit operator Timeout(int value)
            => new Timeout(value);

        public static readonly Timeout NoWait = new Timeout(0);

        public static readonly Timeout Infinite = new Timeout(-1);
    }
}

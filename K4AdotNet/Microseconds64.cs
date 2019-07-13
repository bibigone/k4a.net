using System;

namespace K4AdotNet
{
    public struct Microseconds64 :
        IEquatable<Microseconds64>, IEquatable<TimeSpan>, IEquatable<long>,
        IComparable<Microseconds64>, IComparable<TimeSpan>, IComparable<long>, IComparable,
        IFormattable
    {
        public long ValueUsec;

        public Microseconds64(long valueUsec)
            => ValueUsec = valueUsec;

        public Microseconds64(TimeSpan value)
            => ValueUsec = value.Ticks / UsecToTimeSpanTicksFactor;

        public TimeSpan ToTimeSpan()
            => TimeSpan.FromTicks(ValueUsec * UsecToTimeSpanTicksFactor);

        public double TotalSeconds => ValueUsec / 1_000_000.0;

        public double TotalMilliseconds => ValueUsec / 1_000.0;

        public bool Equals(Microseconds64 other)
            => ValueUsec.Equals(other.ValueUsec);

        public bool Equals(TimeSpan other)
            => Equals(new Microseconds64(other));

        public bool Equals(long otherUsec)
            => ValueUsec.Equals(otherUsec);

        public int CompareTo(Microseconds64 other)
            => ValueUsec.CompareTo(other.ValueUsec);

        public int CompareTo(TimeSpan other)
            => CompareTo(new Microseconds64(other));

        public int CompareTo(long otherUsec)
            => ValueUsec.CompareTo(otherUsec);

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;
            if (obj is Microseconds64)
                return CompareTo((Microseconds64)obj);
            if (obj is TimeSpan)
                return CompareTo((TimeSpan)obj);
            if (obj is IConvertible)
                return CompareTo(Convert.ToInt64(obj));
            throw new ArgumentException("Object is not a Microseconds or TimeSpan or integer number", nameof(obj));
        }

        public string ToString(string format, IFormatProvider formatProvider)
            => ValueUsec.ToString(format, formatProvider) + UNIT_POSTFIX;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is Microseconds64)
                return Equals((Microseconds64)obj);
            if (obj is TimeSpan)
                return Equals((TimeSpan)obj);
            if (obj is IConvertible)
                return Equals(Convert.ToInt64(obj));
            return false;
        }

        public override int GetHashCode()
            => ValueUsec.GetHashCode();

        public override string ToString()
            => ValueUsec.ToString() + UNIT_POSTFIX;

        public static bool operator ==(Microseconds64 left, Microseconds64 right)
            => left.Equals(right);

        public static bool operator !=(Microseconds64 left, Microseconds64 right)
            => !left.Equals(right);

        public static bool operator <(Microseconds64 left, Microseconds64 right)
            => left.CompareTo(right) < 0;

        public static bool operator >(Microseconds64 left, Microseconds64 right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(Microseconds64 left, Microseconds64 right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(Microseconds64 left, Microseconds64 right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(Microseconds64 left, TimeSpan right)
            => left.Equals(right);

        public static bool operator !=(Microseconds64 left, TimeSpan right)
            => !left.Equals(right);

        public static bool operator <(Microseconds64 left, TimeSpan right)
            => left.CompareTo(right) < 0;

        public static bool operator >(Microseconds64 left, TimeSpan right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(Microseconds64 left, TimeSpan right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(Microseconds64 left, TimeSpan right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(TimeSpan left, Microseconds64 right)
            => new Microseconds64(left).Equals(right);

        public static bool operator !=(TimeSpan left, Microseconds64 right)
            => !new Microseconds64(left).Equals(right);

        public static bool operator <(TimeSpan left, Microseconds64 right)
            => new Microseconds64(left).CompareTo(right) < 0;

        public static bool operator >(TimeSpan left, Microseconds64 right)
            => new Microseconds64(left).CompareTo(right) > 0;

        public static bool operator <=(TimeSpan left, Microseconds64 right)
            => new Microseconds64(left).CompareTo(right) <= 0;

        public static bool operator >=(TimeSpan left, Microseconds64 right)
            => new Microseconds64(left).CompareTo(right) >= 0;

        public static bool operator ==(Microseconds64 left, long rightUsec)
            => left.Equals(rightUsec);

        public static bool operator !=(Microseconds64 left, long rightUsec)
            => !left.Equals(rightUsec);

        public static bool operator <(Microseconds64 left, long rightUsec)
            => left.CompareTo(rightUsec) < 0;

        public static bool operator >(Microseconds64 left, long rightUsec)
            => left.CompareTo(rightUsec) > 0;

        public static bool operator <=(Microseconds64 left, long rightUsec)
            => left.CompareTo(rightUsec) <= 0;

        public static bool operator >=(Microseconds64 left, long rightUsec)
            => left.CompareTo(rightUsec) >= 0;

        public static bool operator ==(long leftUsec, Microseconds64 right)
            => new Microseconds64(leftUsec).Equals(right);

        public static bool operator !=(long leftUsec, Microseconds64 right)
            => !new Microseconds64(leftUsec).Equals(right);

        public static bool operator <(long leftUsec, Microseconds64 right)
            => new Microseconds64(leftUsec).CompareTo(right) < 0;

        public static bool operator >(long leftUsec, Microseconds64 right)
            => new Microseconds64(leftUsec).CompareTo(right) > 0;

        public static bool operator <=(long leftUsec, Microseconds64 right)
            => new Microseconds64(leftUsec).CompareTo(right) <= 0;

        public static bool operator >=(long leftUsec, Microseconds64 right)
            => new Microseconds64(leftUsec).CompareTo(right) >= 0;

        public static implicit operator TimeSpan(Microseconds64 value)
            => value.ToTimeSpan();

        public static implicit operator Microseconds64(TimeSpan value)
            => new Microseconds64(value);

        public static implicit operator long(Microseconds64 value)
            => value.ValueUsec;

        public static implicit operator Microseconds64(long valueUsec)
            => new Microseconds64(valueUsec);

        public static Microseconds64 FromSeconds(double valueSec)
            => new Microseconds64((long)(valueSec * 1_000_000));

        public static Microseconds64 FromMilliseconds(double valueMs)
            => new Microseconds64((long)(valueMs * 1_000));

        public static readonly Microseconds64 Zero = new Microseconds64(0);

        internal static readonly long UsecToTimeSpanTicksFactor = TimeSpan.TicksPerSecond / 1_000_000L;
        internal const string UNIT_POSTFIX = " usec";
    }
}

using System;

namespace K4AdotNet
{
    public struct Timestamp :
        IEquatable<Timestamp>, IEquatable<TimeSpan>, IEquatable<long>,
        IComparable<Timestamp>, IComparable<TimeSpan>, IComparable<long>, IComparable,
        IFormattable
    {
        public long ValueUsec;

        public Timestamp(long valueUsec)
            => ValueUsec = valueUsec;

        public Timestamp(TimeSpan value)
            => ValueUsec = value.Ticks / UsecToTimeSpanTicksFactor;

        public TimeSpan ToTimeSpan()
            => TimeSpan.FromTicks(ValueUsec * UsecToTimeSpanTicksFactor);

        public bool Equals(Timestamp other)
            => ValueUsec.Equals(other.ValueUsec);

        public bool Equals(TimeSpan other)
            => Equals(new Timestamp(other));

        public bool Equals(long otherUsec)
            => ValueUsec.Equals(otherUsec);

        public int CompareTo(Timestamp other)
            => ValueUsec.CompareTo(other.ValueUsec);

        public int CompareTo(TimeSpan other)
            => CompareTo(new Timestamp(other));

        public int CompareTo(long otherUsec)
            => ValueUsec.CompareTo(otherUsec);

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;
            if (obj is Timestamp)
                return CompareTo((Timestamp)obj);
            if (obj is TimeSpan)
                return CompareTo((TimeSpan)obj);
            if (obj is IConvertible)
                return CompareTo(Convert.ToInt64(obj));
            throw new ArgumentException("Object is not a Timestamp or TimeSpan or integer number", nameof(obj));
        }

        public string ToString(string format, IFormatProvider formatProvider)
            => ValueUsec.ToString(format, formatProvider) + " usec";

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is Timestamp)
                return Equals((Timestamp)obj);
            if (obj is TimeSpan)
                return Equals((TimeSpan)obj);
            if (obj is IConvertible)
                return Equals(Convert.ToInt64(obj));
            return false;
        }

        public override int GetHashCode()
            => ValueUsec.GetHashCode();

        public override string ToString()
            => ValueUsec.ToString() + " usec";

        public static bool operator ==(Timestamp left, Timestamp right)
            => left.Equals(right);

        public static bool operator !=(Timestamp left, Timestamp right)
            => !left.Equals(right);

        public static bool operator <(Timestamp left, Timestamp right)
            => left.CompareTo(right) < 0;

        public static bool operator >(Timestamp left, Timestamp right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(Timestamp left, Timestamp right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(Timestamp left, Timestamp right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(Timestamp left, TimeSpan right)
            => left.Equals(right);

        public static bool operator !=(Timestamp left, TimeSpan right)
            => !left.Equals(right);

        public static bool operator <(Timestamp left, TimeSpan right)
            => left.CompareTo(right) < 0;

        public static bool operator >(Timestamp left, TimeSpan right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(Timestamp left, TimeSpan right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(Timestamp left, TimeSpan right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(TimeSpan left, Timestamp right)
            => new Timestamp(left).Equals(right);

        public static bool operator !=(TimeSpan left, Timestamp right)
            => !new Timestamp(left).Equals(right);

        public static bool operator <(TimeSpan left, Timestamp right)
            => new Timestamp(left).CompareTo(right) < 0;

        public static bool operator >(TimeSpan left, Timestamp right)
            => new Timestamp(left).CompareTo(right) > 0;

        public static bool operator <=(TimeSpan left, Timestamp right)
            => new Timestamp(left).CompareTo(right) <= 0;

        public static bool operator >=(TimeSpan left, Timestamp right)
            => new Timestamp(left).CompareTo(right) >= 0;

        public static bool operator ==(Timestamp left, long rightUsec)
            => left.Equals(rightUsec);

        public static bool operator !=(Timestamp left, long rightUsec)
            => !left.Equals(rightUsec);

        public static bool operator <(Timestamp left, long rightUsec)
            => left.CompareTo(rightUsec) < 0;

        public static bool operator >(Timestamp left, long rightUsec)
            => left.CompareTo(rightUsec) > 0;

        public static bool operator <=(Timestamp left, long rightUsec)
            => left.CompareTo(rightUsec) <= 0;

        public static bool operator >=(Timestamp left, long rightUsec)
            => left.CompareTo(rightUsec) >= 0;

        public static bool operator ==(long leftUsec, Timestamp right)
            => new Timestamp(leftUsec).Equals(right);

        public static bool operator !=(long leftUsec, Timestamp right)
            => !new Timestamp(leftUsec).Equals(right);

        public static bool operator <(long leftUsec, Timestamp right)
            => new Timestamp(leftUsec).CompareTo(right) < 0;

        public static bool operator >(long leftUsec, Timestamp right)
            => new Timestamp(leftUsec).CompareTo(right) > 0;

        public static bool operator <=(long leftUsec, Timestamp right)
            => new Timestamp(leftUsec).CompareTo(right) <= 0;

        public static bool operator >=(long leftUsec, Timestamp right)
            => new Timestamp(leftUsec).CompareTo(right) >= 0;

        public static implicit operator TimeSpan(Timestamp value)
            => value.ToTimeSpan();

        public static implicit operator Timestamp(TimeSpan value)
            => new Timestamp(value);

        public static implicit operator long(Timestamp value)
            => value.ValueUsec;

        public static implicit operator Timestamp(long valueUsec)
            => new Timestamp(valueUsec);

        public static readonly Timestamp Zero = new Timestamp(0);

        internal static readonly long UsecToTimeSpanTicksFactor = TimeSpan.TicksPerSecond / 1000000L;
    }
}

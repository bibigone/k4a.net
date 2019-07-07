using System;

namespace K4AdotNet
{
    public struct TimeStamp :
        IEquatable<TimeStamp>, IEquatable<TimeSpan>, IEquatable<long>,
        IComparable<TimeStamp>, IComparable<TimeSpan>, IComparable<long>, IComparable,
        IFormattable
    {
        public long ValueUsec;

        public TimeStamp(long valueUsec)
            => ValueUsec = valueUsec;

        public TimeStamp(TimeSpan value)
            => ValueUsec = value.Ticks / UsecToTimeSpanTicksFactor;

        public TimeSpan ToTimeSpan()
            => TimeSpan.FromTicks(ValueUsec * UsecToTimeSpanTicksFactor);

        public bool Equals(TimeStamp other)
            => ValueUsec.Equals(other.ValueUsec);

        public bool Equals(TimeSpan other)
            => Equals(new TimeStamp(other));

        public bool Equals(long otherUsec)
            => ValueUsec.Equals(otherUsec);

        public int CompareTo(TimeStamp other)
            => ValueUsec.CompareTo(other.ValueUsec);

        public int CompareTo(TimeSpan other)
            => CompareTo(new TimeStamp(other));

        public int CompareTo(long otherUsec)
            => ValueUsec.CompareTo(otherUsec);

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;
            if (obj is TimeStamp)
                return CompareTo((TimeStamp)obj);
            if (obj is TimeSpan)
                return CompareTo((TimeSpan)obj);
            if (obj is IConvertible)
                return CompareTo(Convert.ToInt64(obj));
            throw new ArgumentException("Object is not a TimeStamp or TimeSpan or integer number", nameof(obj));
        }

        public string ToString(string format, IFormatProvider formatProvider)
            => ValueUsec.ToString(format, formatProvider) + " usec";

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is TimeStamp)
                return Equals((TimeStamp)obj);
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

        public static bool operator ==(TimeStamp left, TimeStamp right)
            => left.Equals(right);

        public static bool operator !=(TimeStamp left, TimeStamp right)
            => !left.Equals(right);

        public static bool operator <(TimeStamp left, TimeStamp right)
            => left.CompareTo(right) < 0;

        public static bool operator >(TimeStamp left, TimeStamp right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(TimeStamp left, TimeStamp right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(TimeStamp left, TimeStamp right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(TimeStamp left, TimeSpan right)
            => left.Equals(right);

        public static bool operator !=(TimeStamp left, TimeSpan right)
            => !left.Equals(right);

        public static bool operator <(TimeStamp left, TimeSpan right)
            => left.CompareTo(right) < 0;

        public static bool operator >(TimeStamp left, TimeSpan right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(TimeStamp left, TimeSpan right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(TimeStamp left, TimeSpan right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(TimeSpan left, TimeStamp right)
            => new TimeStamp(left).Equals(right);

        public static bool operator !=(TimeSpan left, TimeStamp right)
            => !new TimeStamp(left).Equals(right);

        public static bool operator <(TimeSpan left, TimeStamp right)
            => new TimeStamp(left).CompareTo(right) < 0;

        public static bool operator >(TimeSpan left, TimeStamp right)
            => new TimeStamp(left).CompareTo(right) > 0;

        public static bool operator <=(TimeSpan left, TimeStamp right)
            => new TimeStamp(left).CompareTo(right) <= 0;

        public static bool operator >=(TimeSpan left, TimeStamp right)
            => new TimeStamp(left).CompareTo(right) >= 0;

        public static bool operator ==(TimeStamp left, long rightUsec)
            => left.Equals(rightUsec);

        public static bool operator !=(TimeStamp left, long rightUsec)
            => !left.Equals(rightUsec);

        public static bool operator <(TimeStamp left, long rightUsec)
            => left.CompareTo(rightUsec) < 0;

        public static bool operator >(TimeStamp left, long rightUsec)
            => left.CompareTo(rightUsec) > 0;

        public static bool operator <=(TimeStamp left, long rightUsec)
            => left.CompareTo(rightUsec) <= 0;

        public static bool operator >=(TimeStamp left, long rightUsec)
            => left.CompareTo(rightUsec) >= 0;

        public static bool operator ==(long leftUsec, TimeStamp right)
            => new TimeStamp(leftUsec).Equals(right);

        public static bool operator !=(long leftUsec, TimeStamp right)
            => !new TimeStamp(leftUsec).Equals(right);

        public static bool operator <(long leftUsec, TimeStamp right)
            => new TimeStamp(leftUsec).CompareTo(right) < 0;

        public static bool operator >(long leftUsec, TimeStamp right)
            => new TimeStamp(leftUsec).CompareTo(right) > 0;

        public static bool operator <=(long leftUsec, TimeStamp right)
            => new TimeStamp(leftUsec).CompareTo(right) <= 0;

        public static bool operator >=(long leftUsec, TimeStamp right)
            => new TimeStamp(leftUsec).CompareTo(right) >= 0;

        public static implicit operator TimeSpan(TimeStamp value)
            => value.ToTimeSpan();

        public static implicit operator TimeStamp(TimeSpan value)
            => new TimeStamp(value);

        public static implicit operator long(TimeStamp value)
            => value.ValueUsec;

        public static implicit operator TimeStamp(long valueUsec)
            => new TimeStamp(valueUsec);

        public static readonly TimeStamp Zero = new TimeStamp(0);

        internal static readonly long UsecToTimeSpanTicksFactor = TimeSpan.TicksPerSecond / 1000000L;
    }
}

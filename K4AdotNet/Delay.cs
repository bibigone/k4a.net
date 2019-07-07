using System;

namespace K4AdotNet
{
    public struct Delay :
        IEquatable<Delay>, IEquatable<TimeSpan>, IEquatable<int>,
        IComparable<Delay>, IComparable<TimeSpan>, IComparable<int>, IComparable,
        IFormattable
    {
        public int ValueUsec;

        public Delay(int valueUsec)
            => ValueUsec = valueUsec;

        public Delay(TimeSpan value)
            => ValueUsec = checked((int)(value.Ticks / TimeStamp.UsecToTimeSpanTicksFactor));

        public TimeSpan ToTimeSpan()
            => TimeSpan.FromTicks(ValueUsec * TimeStamp.UsecToTimeSpanTicksFactor);

        public bool Equals(Delay other)
            => ValueUsec.Equals(other.ValueUsec);

        public bool Equals(TimeSpan other)
            => Equals(new Delay(other));

        public bool Equals(int otherUsec)
            => ValueUsec.Equals(otherUsec);

        public int CompareTo(Delay other)
            => ValueUsec.CompareTo(other.ValueUsec);

        public int CompareTo(TimeSpan other)
            => CompareTo(new Delay(other));

        public int CompareTo(int otherUsec)
            => ValueUsec.CompareTo(otherUsec);

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;
            if (obj is Delay)
                return CompareTo((Delay)obj);
            if (obj is TimeSpan)
                return CompareTo((TimeSpan)obj);
            if (obj is IConvertible)
                return CompareTo(Convert.ToInt32(obj));
            throw new ArgumentException("Object is not a Delay or TimeSpan or integer number", nameof(obj));
        }

        public string ToString(string format, IFormatProvider formatProvider)
            => ValueUsec.ToString(format, formatProvider) + " usec";

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is Delay)
                return Equals((Delay)obj);
            if (obj is TimeSpan)
                return Equals((TimeSpan)obj);
            if (obj is IConvertible)
                return Equals(Convert.ToInt32(obj));
            return false;
        }

        public override int GetHashCode()
            => ValueUsec.GetHashCode();

        public override string ToString()
            => ValueUsec.ToString() + " usec";

        public static bool operator ==(Delay left, Delay right)
            => left.Equals(right);

        public static bool operator !=(Delay left, Delay right)
            => !left.Equals(right);

        public static bool operator <(Delay left, Delay right)
            => left.CompareTo(right) < 0;

        public static bool operator >(Delay left, Delay right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(Delay left, Delay right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(Delay left, Delay right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(Delay left, TimeSpan right)
            => left.Equals(right);

        public static bool operator !=(Delay left, TimeSpan right)
            => !left.Equals(right);

        public static bool operator <(Delay left, TimeSpan right)
            => left.CompareTo(right) < 0;

        public static bool operator >(Delay left, TimeSpan right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(Delay left, TimeSpan right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(Delay left, TimeSpan right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(TimeSpan left, Delay right)
            => new Delay(left).Equals(right);

        public static bool operator !=(TimeSpan left, Delay right)
            => !new Delay(left).Equals(right);

        public static bool operator <(TimeSpan left, Delay right)
            => new Delay(left).CompareTo(right) < 0;

        public static bool operator >(TimeSpan left, Delay right)
            => new Delay(left).CompareTo(right) > 0;

        public static bool operator <=(TimeSpan left, Delay right)
            => new Delay(left).CompareTo(right) <= 0;

        public static bool operator >=(TimeSpan left, Delay right)
            => new Delay(left).CompareTo(right) >= 0;

        public static bool operator ==(Delay left, int rightUsec)
            => left.Equals(rightUsec);

        public static bool operator !=(Delay left, int rightUsec)
            => !left.Equals(rightUsec);

        public static bool operator <(Delay left, int rightUsec)
            => left.CompareTo(rightUsec) < 0;

        public static bool operator >(Delay left, int rightUsec)
            => left.CompareTo(rightUsec) > 0;

        public static bool operator <=(Delay left, int rightUsec)
            => left.CompareTo(rightUsec) <= 0;

        public static bool operator >=(Delay left, int rightUsec)
            => left.CompareTo(rightUsec) >= 0;

        public static bool operator ==(int leftUsec, Delay right)
            => new Delay(leftUsec).Equals(right);

        public static bool operator !=(int leftUsec, Delay right)
            => !new Delay(leftUsec).Equals(right);

        public static bool operator <(int leftUsec, Delay right)
            => new Delay(leftUsec).CompareTo(right) < 0;

        public static bool operator >(int leftUsec, Delay right)
            => new Delay(leftUsec).CompareTo(right) > 0;

        public static bool operator <=(int leftUsec, Delay right)
            => new Delay(leftUsec).CompareTo(right) <= 0;

        public static bool operator >=(int leftUsec, Delay right)
            => new Delay(leftUsec).CompareTo(right) >= 0;

        public static implicit operator TimeSpan(Delay value)
            => value.ToTimeSpan();

        public static implicit operator Delay(TimeSpan value)
            => new Delay(value);

        public static implicit operator int(Delay value)
            => value.ValueUsec;

        public static implicit operator Delay(int valueUsec)
            => new Delay(valueUsec);

        public static readonly Delay Zero = new Delay(0);
    }
}

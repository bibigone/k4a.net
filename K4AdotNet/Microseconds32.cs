using System;

namespace K4AdotNet
{
    public struct Microseconds32 :
        IEquatable<Microseconds32>, IEquatable<TimeSpan>, IEquatable<int>,
        IComparable<Microseconds32>, IComparable<TimeSpan>, IComparable<int>, IComparable,
        IFormattable
    {
        public int ValueUsec;

        public Microseconds32(int valueUsec)
            => ValueUsec = valueUsec;

        public Microseconds32(TimeSpan value)
            => ValueUsec = checked((int)(value.Ticks / Microseconds64.UsecToTimeSpanTicksFactor));

        public TimeSpan ToTimeSpan()
            => TimeSpan.FromTicks(ValueUsec * Microseconds64.UsecToTimeSpanTicksFactor);

        public double TotalSeconds => ValueUsec / 1_000_000.0;

        public double TotalMilliseconds => ValueUsec / 1_000.0;

        public bool Equals(Microseconds32 other)
            => ValueUsec.Equals(other.ValueUsec);

        public bool Equals(TimeSpan other)
            => Equals(new Microseconds32(other));

        public bool Equals(int otherUsec)
            => ValueUsec.Equals(otherUsec);

        public int CompareTo(Microseconds32 other)
            => ValueUsec.CompareTo(other.ValueUsec);

        public int CompareTo(TimeSpan other)
            => CompareTo(new Microseconds32(other));

        public int CompareTo(int otherUsec)
            => ValueUsec.CompareTo(otherUsec);

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;
            if (obj is Microseconds32)
                return CompareTo((Microseconds32)obj);
            if (obj is TimeSpan)
                return CompareTo((TimeSpan)obj);
            if (obj is IConvertible)
                return CompareTo(Convert.ToInt32(obj));
            throw new ArgumentException("Object is not a Microseconds32 or TimeSpan or integer number", nameof(obj));
        }

        public string ToString(string format, IFormatProvider formatProvider)
            => ValueUsec.ToString(format, formatProvider) + Microseconds64.UNIT_POSTFIX;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is Microseconds32)
                return Equals((Microseconds32)obj);
            if (obj is TimeSpan)
                return Equals((TimeSpan)obj);
            if (obj is IConvertible)
                return Equals(Convert.ToInt32(obj));
            return false;
        }

        public override int GetHashCode()
            => ValueUsec.GetHashCode();

        public override string ToString()
            => ValueUsec.ToString() + Microseconds64.UNIT_POSTFIX;

        public static bool operator ==(Microseconds32 left, Microseconds32 right)
            => left.Equals(right);

        public static bool operator !=(Microseconds32 left, Microseconds32 right)
            => !left.Equals(right);

        public static bool operator <(Microseconds32 left, Microseconds32 right)
            => left.CompareTo(right) < 0;

        public static bool operator >(Microseconds32 left, Microseconds32 right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(Microseconds32 left, Microseconds32 right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(Microseconds32 left, Microseconds32 right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(Microseconds32 left, TimeSpan right)
            => left.Equals(right);

        public static bool operator !=(Microseconds32 left, TimeSpan right)
            => !left.Equals(right);

        public static bool operator <(Microseconds32 left, TimeSpan right)
            => left.CompareTo(right) < 0;

        public static bool operator >(Microseconds32 left, TimeSpan right)
            => left.CompareTo(right) > 0;

        public static bool operator <=(Microseconds32 left, TimeSpan right)
            => left.CompareTo(right) <= 0;

        public static bool operator >=(Microseconds32 left, TimeSpan right)
            => left.CompareTo(right) >= 0;

        public static bool operator ==(TimeSpan left, Microseconds32 right)
            => new Microseconds32(left).Equals(right);

        public static bool operator !=(TimeSpan left, Microseconds32 right)
            => !new Microseconds32(left).Equals(right);

        public static bool operator <(TimeSpan left, Microseconds32 right)
            => new Microseconds32(left).CompareTo(right) < 0;

        public static bool operator >(TimeSpan left, Microseconds32 right)
            => new Microseconds32(left).CompareTo(right) > 0;

        public static bool operator <=(TimeSpan left, Microseconds32 right)
            => new Microseconds32(left).CompareTo(right) <= 0;

        public static bool operator >=(TimeSpan left, Microseconds32 right)
            => new Microseconds32(left).CompareTo(right) >= 0;

        public static bool operator ==(Microseconds32 left, int rightUsec)
            => left.Equals(rightUsec);

        public static bool operator !=(Microseconds32 left, int rightUsec)
            => !left.Equals(rightUsec);

        public static bool operator <(Microseconds32 left, int rightUsec)
            => left.CompareTo(rightUsec) < 0;

        public static bool operator >(Microseconds32 left, int rightUsec)
            => left.CompareTo(rightUsec) > 0;

        public static bool operator <=(Microseconds32 left, int rightUsec)
            => left.CompareTo(rightUsec) <= 0;

        public static bool operator >=(Microseconds32 left, int rightUsec)
            => left.CompareTo(rightUsec) >= 0;

        public static bool operator ==(int leftUsec, Microseconds32 right)
            => new Microseconds32(leftUsec).Equals(right);

        public static bool operator !=(int leftUsec, Microseconds32 right)
            => !new Microseconds32(leftUsec).Equals(right);

        public static bool operator <(int leftUsec, Microseconds32 right)
            => new Microseconds32(leftUsec).CompareTo(right) < 0;

        public static bool operator >(int leftUsec, Microseconds32 right)
            => new Microseconds32(leftUsec).CompareTo(right) > 0;

        public static bool operator <=(int leftUsec, Microseconds32 right)
            => new Microseconds32(leftUsec).CompareTo(right) <= 0;

        public static bool operator >=(int leftUsec, Microseconds32 right)
            => new Microseconds32(leftUsec).CompareTo(right) >= 0;

        public static implicit operator TimeSpan(Microseconds32 value)
            => value.ToTimeSpan();

        public static implicit operator Microseconds32(TimeSpan value)
            => new Microseconds32(value);

        public static implicit operator int(Microseconds32 value)
            => value.ValueUsec;

        public static implicit operator Microseconds32(int valueUsec)
            => new Microseconds32(valueUsec);

        public static Microseconds32 FromSeconds(double valueSec)
            => new Microseconds32((int)(valueSec * 1_000_000));

        public static Microseconds32 FromMilliseconds(double valueMs)
            => new Microseconds32((int)(valueMs * 1_000));

        public static readonly Microseconds32 Zero = new Microseconds32(0);
    }
}

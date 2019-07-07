using System;
using System.Runtime.InteropServices;

namespace K4AdotNet
{
    // Defined in k4atypes.h:
    // typedef union
    // {
    //     struct _xy
    //     {
    //         float x; /**< X component of a vector. */
    //         float y; /**< Y component of a vector. */
    //     } xy;       /**< X, Y representation of a vector. */
    //     float v[2];  /**< Array representation of a vector. */
    // } k4a_float2_t;
    /// <summary>Two dimensional floating point vector.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Float2 : IEquatable<Float2>, IFormattable
    {
        /// <summary>X component of a vector. Corresponds to <c>0</c> index in array representation.</summary>
        public float X;

        /// <summary>Y component of a vector. Corresponds to <c>1</c> index in array representation.</summary>
        public float Y;

        /// <summary>Constructs vector with given components.</summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        public Float2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Float2(float[] values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(values) + "." + nameof(values.Length));
            X = values[0];
            Y = values[1];
        }

        public float[] ToArray()
            => new[] { X, Y };

        /// <summary>Indexed access to vector components.</summary>
        /// <param name="index">Index of component: <c>X</c> - <c>0</c>, <c>Y</c> - <c>1</c>.</param>
        /// <returns>Value of appropriate component.</returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        /// <summary>Per-component comparison.</summary>
        /// <param name="other">Other vector to be compared to this one.</param>
        /// <returns><c>true</c> if all components are equal.</returns>
        public bool Equals(Float2 other)
            => X.Equals(other.X) && Y.Equals(other.Y);

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Float2))
                return false;
            return Equals((Float2)obj);
        }

        public static bool operator ==(Float2 left, Float2 right)
            => left.Equals(right);

        public static bool operator !=(Float2 left, Float2 right)
            => !left.Equals(right);

        public override int GetHashCode()
            => X.GetHashCode() ^ Y.GetHashCode();

        /// <summary>Formats vector as <c>[X Y]</c> string.</summary>
        /// <param name="format">Format string for each individual component in string representation.</param>
        /// <param name="formatProvider">Culture for formatting numbers to strings.</param>
        /// <returns>String representation of vector in a given Culture.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => $"[{X.ToString(format, formatProvider)} {Y.ToString(format, formatProvider)}]";

        public override string ToString()
            => $"[{X} {Y}]";

        /// <summary>Zero vector.</summary>
        public static readonly Float2 Zero = new Float2();

        /// <summary>Unit vector in +X direction.</summary>
        public static readonly Float2 UnitX = new Float2(1, 0);

        /// <summary>Unit vector in +Y direction.</summary>
        public static readonly Float2 UnitY = new Float2(0, 1);
    }
}

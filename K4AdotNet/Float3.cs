using System;
using System.Runtime.InteropServices;

namespace K4AdotNet
{
    // Defined in k4atypes.h:
    // typedef union
    // {
    //     struct _xyz
    //     {
    //         float x; /**< X component of a vector. */
    //         float y; /**< Y component of a vector. */
    //         float z; /**< Z component of a vector. */
    //     } xyz;       /**< X, Y, Z representation of a vector. */
    //     float v[3];  /**< Array representation of a vector. */
    // } k4a_float3_t;
    /// <summary>X, Y, Z representation of a vector.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Float3 : IEquatable<Float3>, IFormattable
    {
        /// <summary>X component of a vector. Corresponds to <c>0</c> index in array representation.</summary>
        public float X;

        /// <summary>Y component of a vector. Corresponds to <c>1</c> index in array representation.</summary>
        public float Y;

        /// <summary>Z component of a vector. Corresponds to <c>2</c> index in array representation.</summary>
        public float Z;

        /// <summary>Constructs vector with given components.</summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        public Float3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Float3(float[] values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 3)
                throw new ArgumentOutOfRangeException(nameof(values) + "." + nameof(values.Length));
            X = values[0];
            Y = values[1];
            Z = values[2];
        }

        public float[] ToArray()
            => new[] { X, Y, Z };

        /// <summary>Indexed access to vector components.</summary>
        /// <param name="index">Index of component: <c>X</c> - <c>0</c>, <c>Y</c> - <c>1</c>, <c>Z</c> - <c>2</c>.</param>
        /// <returns>Value of appropriate component.</returns>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        /// <summary>Per-component comparison.</summary>
        /// <param name="other">Other vector to be compared to this one.</param>
        /// <returns><c>true</c> if all components are equal.</returns>
        public bool Equals(Float3 other)
            => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Float3))
                return false;
            return Equals((Float3)obj);
        }

        public static bool operator ==(Float3 left, Float3 right)
            => left.Equals(right);

        public static bool operator !=(Float3 left, Float3 right)
            => !left.Equals(right);

        public override int GetHashCode()
            => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

        /// <summary>Formats vector as <c>[X Y Z]</c> string.</summary>
        /// <param name="format">Format string for each individual component in string representation.</param>
        /// <param name="formatProvider">Culture for formatting numbers to strings.</param>
        /// <returns>String representation of vector in a given Culture.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => $"[{X.ToString(format, formatProvider)} {Y.ToString(format, formatProvider)} {Z.ToString(format, formatProvider)}]";

        public override string ToString()
            => $"[{X} {Y} {Z}]";

        /// <summary>Zero vector.</summary>
        public static readonly Float3 Zero = new Float3();

        /// <summary>Unit vector in +X direction.</summary>
        public static readonly Float3 UnitX = new Float3(1, 0, 0);

        /// <summary>Unit vector in +Y direction.</summary>
        public static readonly Float3 UnitY = new Float3(0, 1, 0);

        /// <summary>Unit vector in +Z direction.</summary>
        public static readonly Float3 UnitZ = new Float3(0, 0, 1);
    }
}

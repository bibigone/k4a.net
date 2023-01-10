using System;
using System.Runtime.InteropServices;

namespace K4AdotNet
{
    // Defined in k4atypes.h:
    // typedef union
    // {
    //     struct _xyz
    //     {
    //         float x;
    //         float y;
    //         float z;
    //     } xyz;
    //     float v[3];
    // } k4a_float3_t;
    //
    /// <summary>Three dimensional floating point vector.</summary>
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

        /// <summary>Creates vector structure from array representation.</summary>
        /// <param name="xyz">Array representation of vector. Not <see langword="null"/>. Two elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="xyz"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Wrong length of <paramref name="xyz"/> array.</exception>
        public Float3(float[] xyz)
        {
            if (xyz is null)
                throw new ArgumentNullException(nameof(xyz));
            if (xyz.Length != 3)
                throw new ArgumentOutOfRangeException(nameof(xyz) + "." + nameof(xyz.Length));
            X = xyz[0];
            Y = xyz[1];
            Z = xyz[2];
        }

        /// <summary>Converts vector structure to array representation.</summary>
        /// <returns>Array representation of vector. Not <see langword="null"/>.</returns>
        public float[] ToArray()
            => new[] { X, Y, Z };

        /// <summary>Indexed access to vector components.</summary>
        /// <param name="index">Index of component: <see cref="X"/> - <c>0</c>, <see cref="Y"/> - <c>1</c>, <see cref="Z"/> - <c>2</c>.</param>
        /// <returns>Value of appropriate component.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> has invalid value.</exception>
        public float this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException(),
            };

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Per-component comparison.</summary>
        /// <param name="other">Other vector to be compared to this one.</param>
        /// <returns><see langword="true"/> if all components of <paramref name="other"/> are equal to appropriate components of this vector.</returns>
        public bool Equals(Float3 other)
            => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

        /// <summary>Overloads <see cref="Object.Equals(object)"/> to be consistent with <see cref="Equals(Float3)"/>.</summary>
        /// <param name="obj">Object to be compared with this vector.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Float3"/> and is equal to this one.</returns>
        /// <seealso cref="Equals(Float3)"/>
        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Float3 float3)
                return false;
            return Equals(float3);
        }

        /// <summary>To be consistent with <see cref="Equals(Float3)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Float3)"/>
        public static bool operator ==(Float3 left, Float3 right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Float3)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Float3)"/>
        public static bool operator !=(Float3 left, Float3 right)
            => !left.Equals(right);

        /// <summary>Calculates hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        public override int GetHashCode()
            => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

        /// <summary>Formats vector as <c>[X Y Z]</c> string.</summary>
        /// <param name="format">Format string for each individual component in string representation.</param>
        /// <param name="formatProvider">Culture for formatting numbers to strings.</param>
        /// <returns>String representation of vector in a given Culture.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => $"[{X.ToString(format, formatProvider)} {Y.ToString(format, formatProvider)} {Z.ToString(format, formatProvider)}]";

        /// <summary>Formats vector as <c>[X Y Z]</c> string.</summary>
        /// <returns><c>[X Y Z]</c>.</returns>
        public override string ToString()
            => $"[{X} {Y} {Z}]";

        /// <summary>Zero vector.</summary>
        public static readonly Float3 Zero = new();

        /// <summary>Unit vector in +X direction.</summary>
        public static readonly Float3 UnitX = new(1f, 0f, 0f);

        /// <summary>Unit vector in +Y direction.</summary>
        public static readonly Float3 UnitY = new(0f, 1f, 0f);

        /// <summary>Unit vector in +Z direction.</summary>
        public static readonly Float3 UnitZ = new(0f, 0f, 1f);
    }
}

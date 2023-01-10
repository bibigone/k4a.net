using System;
using System.Runtime.InteropServices;

namespace K4AdotNet
{
    // Defined in k4atypes.h:
    // typedef union
    // {
    //     struct _xy
    //     {
    //         float x;
    //         float y;
    //     } xy;
    //     float v[2];
    // } k4a_float2_t;
    //
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

        /// <summary>Creates vector structure from array representation.</summary>
        /// <param name="xy">Array representation of vector. Not <see langword="null"/>. Two elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="xy"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Wrong length of <paramref name="xy"/> array.</exception>
        public Float2(float[] xy)
        {
            if (xy is null)
                throw new ArgumentNullException(nameof(xy));
            if (xy.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(xy) + "." + nameof(xy.Length));
            X = xy[0];
            Y = xy[1];
        }

        /// <summary>Converts vector structure to array representation.</summary>
        /// <returns>Array representation of vector. Not <see langword="null"/>.</returns>
        public float[] ToArray()
            => new[] { X, Y };

        /// <summary>Indexed access to vector components.</summary>
        /// <param name="index">Index of component: <see cref="X"/> - <c>0</c>, <see cref="Y"/> - <c>1</c>.</param>
        /// <returns>Value of appropriate component.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> has invalid value.</exception>
        public float this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                _ => throw new IndexOutOfRangeException(),
            };

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    default: throw new IndexOutOfRangeException(nameof(index));
                }
            }
        }

        /// <summary>Per-component comparison.</summary>
        /// <param name="other">Other vector to be compared to this one.</param>
        /// <returns><see langword="true"/> if all components are equal.</returns>
        public bool Equals(Float2 other)
            => X.Equals(other.X) && Y.Equals(other.Y);

        /// <summary>Overloads <see cref="Object.Equals(object)"/> to be consistent with <see cref="Equals(Float2)"/>.</summary>
        /// <param name="obj">Object to be compared with this vector.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Float2"/> and is equal to this one.</returns>
        /// <seealso cref="Equals(Float2)"/>
        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Float2 float2)
                return false;
            return Equals(float2);
        }

        /// <summary>To be consistent with <see cref="Equals(Float2)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Float2)"/>
        public static bool operator ==(Float2 left, Float2 right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Float2)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Float2)"/>
        public static bool operator !=(Float2 left, Float2 right)
            => !left.Equals(right);

        /// <summary>Calculates hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        public override int GetHashCode()
            => X.GetHashCode() ^ Y.GetHashCode();

        /// <summary>Formats vector as <c>[X Y]</c> string.</summary>
        /// <param name="format">Format string for each individual component in string representation.</param>
        /// <param name="formatProvider">Culture for formatting numbers to strings.</param>
        /// <returns>String representation of vector in a given Culture.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => $"[{X.ToString(format, formatProvider)} {Y.ToString(format, formatProvider)}]";

        /// <summary>Formats vector as <c>[X Y]</c> string.</summary>
        /// <returns><c>[X Y]</c>.</returns>
        public override string ToString()
            => $"[{X} {Y}]";

        /// <summary>Zero vector.</summary>
        public static readonly Float2 Zero = new();

        /// <summary>Unit vector in +X direction.</summary>
        public static readonly Float2 UnitX = new(1f, 0f);

        /// <summary>Unit vector in +Y direction.</summary>
        public static readonly Float2 UnitY = new(0f, 1f);
    }
}

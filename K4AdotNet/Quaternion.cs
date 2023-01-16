using System;
using System.Runtime.InteropServices;

namespace K4AdotNet
{
    // Defined in k4abttypes.h:
    // typedef union
    // {
    //     struct _wxyz
    //     {
    //         float w;
    //         float x;
    //         float y;
    //         float z;
    //     } wxyz;
    //     float v[4];
    // } k4a_quaternion_t;
    //
    /// <summary>WXYZ representation of quaternion. Quaternions are used to represent rotations in 3D.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion : IEquatable<Quaternion>, IFormattable
    {
        #region Fields

        /// <summary>W component of a quaternion. Corresponds to <c>0</c> index in array representation.</summary>
        public float W;

        /// <summary>X component of a quaternion. Corresponds to <c>1</c> index in array representation.</summary>
        public float X;

        /// <summary>Y component of a quaternion. Corresponds to <c>2</c> index in array representation.</summary>
        public float Y;

        /// <summary>Z component of a quaternion. Corresponds to <c>3</c> index in array representation.</summary>
        public float Z;

        #endregion

        /// <summary>Creates quaternion with specified components.</summary>
        /// <param name="w">Value for <see cref="W"/> component of quaternion.</param>
        /// <param name="x">Value for <see cref="X"/> component of quaternion.</param>
        /// <param name="y">Value for <see cref="Y"/> component of quaternion.</param>
        /// <param name="z">Value for <see cref="Z"/> component of quaternion.</param>
        public Quaternion(float w, float x, float y, float z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>Creates quaternion with specified components.</summary>
        /// <param name="w">Value for <see cref="W"/> component of quaternion.</param>
        /// <param name="v">Value for <see cref="X"/>, <see cref="Y"/> and <see cref="Z"/> components of quaternion.</param>
        public Quaternion(float w, Float3 v)
        {
            W = w;
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>Creates quaternion from array representation.</summary>
        /// <param name="wxyz">Values for quaternion components in W, X, Y, Z order. Not <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="wxyz"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Wrong length of <paramref name="wxyz"/> array.</exception>
        public Quaternion(float[] wxyz)
        {
            if (wxyz is null)
                throw new ArgumentNullException(nameof(wxyz));
            if (wxyz.Length != 4)
                throw new ArgumentOutOfRangeException(nameof(wxyz) + "." + nameof(wxyz.Length));

            W = wxyz[0];
            X = wxyz[1];
            Y = wxyz[2];
            Z = wxyz[3];
        }

        /// <summary>Converts quaternion to array representation.</summary>
        /// <returns>Array representation of quaternion. Not <see langword="null"/>.</returns>
        public float[] ToArray()
            => new[] { W, X, Y, Z };

        /// <summary>Indexed access to quaternion components.</summary>
        /// <param name="index">Index of component: <see cref="W"/> - <c>0</c>, <see cref="X"/> - <c>1</c>, <see cref="Y"/> - <c>2</c>, <see cref="Z"/> - <c>3</c>.</param>
        /// <returns>Value of appropriate component.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> has invalid value.</exception>
        public float this[int index]
        {
            get => index switch
            {
                0 => W,
                1 => X,
                2 => Y,
                3 => Z,
                _ => throw new IndexOutOfRangeException(),
            };

            set
            {
                switch (index)
                {
                    case 0: W = value; break;
                    case 1: X = value; break;
                    case 2: Y = value; break;
                    case 3: Z = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Per-component comparison.</summary>
        /// <param name="other">Other quaternion to be compared to this one.</param>
        /// <returns><see langword="true"/> if all components of <paramref name="other"/> are equal to appropriate components of this quaternion.</returns>
        public bool Equals(Quaternion other)
            => W.Equals(other.W) && X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

        /// <summary>Overloads <see cref="Object.Equals(object)"/> to be consistent with <see cref="Equals(Quaternion)"/>.</summary>
        /// <param name="obj">Object to be compared with this quaternion.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Quaternion"/> and is equal to this one.</returns>
        /// <seealso cref="Equals(Quaternion)"/>
        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Quaternion q)
                return false;
            return Equals(q);
        }

        /// <summary>To be consistent with <see cref="Equals(Quaternion)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Quaternion)"/>
        public static bool operator ==(Quaternion left, Quaternion right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Quaternion)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Quaternion)"/>
        public static bool operator !=(Quaternion left, Quaternion right)
            => !left.Equals(right);

        /// <summary>Calculates hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        public override int GetHashCode()
            => W.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

        /// <summary>Formats quaternion as <c>[W X Y Z]</c> string.</summary>
        /// <param name="format">Format string for each individual component in string representation.</param>
        /// <param name="formatProvider">Culture for formatting numbers to strings.</param>
        /// <returns>String representation of quaternion in a given Culture.</returns>
        public string ToString(string? format, IFormatProvider? formatProvider)
            => $"[W:{W.ToString(format, formatProvider)} X:{X.ToString(format, formatProvider)} Y:{Y.ToString(format, formatProvider)} Z:{Z.ToString(format, formatProvider)}]";

        /// <summary>Formats quaternion as <c>[W X Y Z]</c> string.</summary>
        /// <returns><c>[W X Y Z]</c>.</returns>
        public override string ToString()
            => $"[W:{W} X:{X} Y:{Y} Z:{Z}]";

        /// <summary>Zero quaternion (all components are 0).</summary>
        public static readonly Quaternion Zero = new();

        /// <summary>Identity quaternion (represents zero rotation in 3D).</summary>
        public static readonly Quaternion Identity = new(1f, 0f, 0f, 0f);
    }
}

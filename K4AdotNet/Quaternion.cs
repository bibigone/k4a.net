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
    /// <summary>WXYZ representation of quaternion.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion : IEquatable<Quaternion>, IFormattable
    {
        /// <summary>W component of a vector. Corresponds to <c>0</c> index in array representation.</summary>
        public float W;

        /// <summary>X component of a vector. Corresponds to <c>1</c> index in array representation.</summary>
        public float X;

        /// <summary>Y component of a vector. Corresponds to <c>2</c> index in array representation.</summary>
        public float Y;

        /// <summary>Z component of a vector. Corresponds to <c>3</c> index in array representation.</summary>
        public float Z;

        public Quaternion(float w, float x, float y, float z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        public Quaternion(float w, Float3 v)
        {
            W = w;
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

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

        public float[] ToArray()
            => new[] { W, X, Y, Z };

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return W;
                    case 1: return X;
                    case 2: return Y;
                    case 3: return Z;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }

            set
            {
                switch (index)
                {
                    case 0: W = value; break;
                    case 1: X = value; break;
                    case 2: Y = value; break;
                    case 3: Z = value; break;
                }
            }
        }

        public bool Equals(Quaternion other)
            => W.Equals(other.W) && X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Quaternion))
                return false;
            return Equals((Quaternion)obj);
        }

        public static bool operator ==(Quaternion left, Quaternion right)
            => left.Equals(right);

        public static bool operator !=(Quaternion left, Quaternion right)
            => !left.Equals(right);

        public override int GetHashCode()
            => W.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

        public string ToString(string format, IFormatProvider formatProvider)
            => $"[W:{W.ToString(format, formatProvider)} X:{X.ToString(format, formatProvider)} Y:{Y.ToString(format, formatProvider)} Z:{Z.ToString(format, formatProvider)}]";

        public override string ToString()
            => $"[W:{W} X:{X} Y:{Y} Z:{Z}]";

        public static readonly Quaternion Zero = new Quaternion();

        public static readonly Quaternion Identity = new Quaternion(1f, 0f, 0f, 0f);
    }
}

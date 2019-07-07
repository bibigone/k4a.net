using System;
using System.Runtime.InteropServices;

namespace K4AdotNet
{
    // In k4atypes.h it is represented simply as float[9] array.
    /// <summary>Placeholder for 3x3 matrix data.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Float3x3 : IEquatable<Float3x3>
    {
        public float M11, M12, M13;
        public float M21, M22, M23;
        public float M31, M32, M33;

        public Float3x3(
            float m11, float m12, float m13,
            float m21, float m22, float m23,
            float m31, float m32, float m33)
        {
            M11 = m11; M12 = m12; M13 = m13;
            M21 = m21; M22 = m22; M23 = m23;
            M31 = m31; M32 = m32; M33 = m33;
        }

        public Float3x3(float[] values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length != 9)
                throw new ArgumentOutOfRangeException(nameof(values) + "." + nameof(values.Length));
            M11 = values[0]; M12 = values[1]; M13 = values[2];
            M21 = values[3]; M22 = values[4]; M23 = values[5];
            M31 = values[6]; M32 = values[7]; M33 = values[8];
        }

        public float[] ToArray()
            => new[] { M11, M12, M13, M21, M22, M23, M31, M32, M33 };

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return M11;
                    case 1: return M12;
                    case 2: return M13;
                    case 3: return M21;
                    case 4: return M22;
                    case 5: return M23;
                    case 6: return M31;
                    case 7: return M32;
                    case 8: return M33;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }

            set
            {
                switch (index)
                {
                    case 0: M11 = value; break;
                    case 1: M12 = value; break;
                    case 2: M13 = value; break;
                    case 3: M21 = value; break;
                    case 4: M22 = value; break;
                    case 5: M23 = value; break;
                    case 6: M31 = value; break;
                    case 7: M32 = value; break;
                    case 8: M33 = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        public float this[int row, int column]
        {
            get
            {
                if (row < 0 || row > 2)
                    throw new ArgumentOutOfRangeException(nameof(row));
                if (column < 0 || column > 2)
                    throw new ArgumentOutOfRangeException(nameof(column));

                return this[(row * 3) + column];
            }

            set
            {
                if (row < 0 || row > 2)
                    throw new ArgumentOutOfRangeException(nameof(row));
                if (column < 0 || column > 2)
                    throw new ArgumentOutOfRangeException(nameof(column));

                this[(row * 3) + column] = value;
            }
        }

        public bool Equals(Float3x3 other)
            => M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13)
            && M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23)
            && M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33);

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Float3x3))
                return false;
            return Equals((Float3x3)obj);
        }

        public static bool operator ==(Float3x3 left, Float3x3 right)
            => left.Equals(right);

        public static bool operator !=(Float3x3 left, Float3x3 right)
            => !left.Equals(right);

        public override int GetHashCode()
            => M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode()
             ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode()
             ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode();

        public static readonly Float3x3 Zero = new Float3x3();

        public static readonly Float3x3 Identity = new Float3x3 { M11 = 1f, M22 = 1f, M33 = 1f };
    }
}

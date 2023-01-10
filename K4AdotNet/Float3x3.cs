using System;
using System.Runtime.InteropServices;

namespace K4AdotNet
{
    // In k4atypes.h it is represented simply as float[9] array.
    //
    /// <summary>Placeholder for 3x3 matrix data.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Float3x3 : IEquatable<Float3x3>, IFormattable
    {
        #region Fields

        /// <summary>First row, first column.</summary>
        public float M11;

        /// <summary>First row, second column.</summary>
        public float M12;

        /// <summary>First row, third column.</summary>
        public float M13;

        /// <summary>Second row, first column.</summary>
        public float M21;

        /// <summary>Second row, second column.</summary>
        public float M22;

        /// <summary>Second row, third column.</summary>
        public float M23;

        /// <summary>Third row, first column.</summary>
        public float M31;

        /// <summary>Third row, second column.</summary>
        public float M32;

        /// <summary>Third row, third column.</summary>
        public float M33;

        #endregion

        /// <summary>Creates matrix initialized by specified values.</summary>
        /// <param name="m11">Value for <see cref="M11"/> (first row, first column).</param>
        /// <param name="m12">Value for <see cref="M12"/> (first row, second column).</param>
        /// <param name="m13">Value for <see cref="M13"/> (first row, third column).</param>
        /// <param name="m21">Value for <see cref="M21"/> (second row, first column).</param>
        /// <param name="m22">Value for <see cref="M22"/> (second row, second column).</param>
        /// <param name="m23">Value for <see cref="M23"/> (second row, third column).</param>
        /// <param name="m31">Value for <see cref="M31"/> (third row, first column).</param>
        /// <param name="m32">Value for <see cref="M32"/> (third row, second column).</param>
        /// <param name="m33">Value for <see cref="M33"/> (third row, thirst column).</param>
        public Float3x3(
            float m11, float m12, float m13,
            float m21, float m22, float m23,
            float m31, float m32, float m33)
        {
            M11 = m11; M12 = m12; M13 = m13;
            M21 = m21; M22 = m22; M23 = m23;
            M31 = m31; M32 = m32; M33 = m33;
        }

        /// <summary>Creates matrix from array representation.</summary>
        /// <param name="values">Array representation of matrix. Not <see langword="null"/>. 9 elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> equals to <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Wrong length of <paramref name="values"/> array.</exception>
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

        /// <summary>Converts matrix structure to array representation.</summary>
        /// <returns>Array representation of matrix. Not <see langword="null"/>.</returns>
        public float[] ToArray()
            => new[] { M11, M12, M13, M21, M22, M23, M31, M32, M33 };

        /// <summary>One dimensional indexed access to matrix components.</summary>
        /// <param name="index">Index of matrix component.</param>
        /// <returns>Matrix element.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> has invalid value.</exception>
        public float this[int index]
        {
            get => index switch
            {
                0 => M11,
                1 => M12,
                2 => M13,
                3 => M21,
                4 => M22,
                5 => M23,
                6 => M31,
                7 => M32,
                8 => M33,
                _ => throw new IndexOutOfRangeException(),
            };

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
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>Two dimensional indexed access to matrix components.</summary>
        /// <param name="rowIndex">Zero-based row index.</param>
        /// <param name="columnIndex">Zero-based row index.</param>
        /// <returns>Matrix element.</returns>
        /// <exception cref="IndexOutOfRangeException"><paramref name="rowIndex"/> or <paramref name="columnIndex"/> has invalid value.</exception>
        public float this[int rowIndex, int columnIndex]
        {
            get
            {
                if (rowIndex < 0 || rowIndex > 2)
                    throw new IndexOutOfRangeException();
                if (columnIndex < 0 || columnIndex > 2)
                    throw new IndexOutOfRangeException();

                return this[(rowIndex * 3) + columnIndex];
            }

            set
            {
                if (rowIndex < 0 || rowIndex > 2)
                    throw new IndexOutOfRangeException();
                if (columnIndex < 0 || columnIndex > 2)
                    throw new IndexOutOfRangeException();

                this[(rowIndex * 3) + columnIndex] = value;
            }
        }

        /// <summary>Per-elements comparison.</summary>
        /// <param name="other">Another matrix to be compared with this one.</param>
        /// <returns><see langword="true"/> if all elements of <paramref name="other"/> are equal to appropriate elements of this matrix.</returns>
        public bool Equals(Float3x3 other)
            => M11.Equals(other.M11) && M12.Equals(other.M12) && M13.Equals(other.M13)
            && M21.Equals(other.M21) && M22.Equals(other.M22) && M23.Equals(other.M23)
            && M31.Equals(other.M31) && M32.Equals(other.M32) && M33.Equals(other.M33);

        /// <summary>Overloads <see cref="Object.Equals(object)"/> to be consistent with <see cref="Equals(Float3x3)"/>.</summary>
        /// <param name="obj">Object to be compared with this matrix.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Float3x3"/> and is equal to this one.</returns>
        /// <seealso cref="Equals(Float3x3)"/>
        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not Float3x3 float33)
                return false;
            return Equals(float33);
        }

        /// <summary>To be consistent with <see cref="Equals(Float3x3)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> equals to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Float3x3)"/>
        public static bool operator ==(Float3x3 left, Float3x3 right)
            => left.Equals(right);

        /// <summary>To be consistent with <see cref="Equals(Float3x3)"/>.</summary>
        /// <param name="left">Left part of operator.</param>
        /// <param name="right">Right part of operator.</param>
        /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        /// <seealso cref="Equals(Float3x3)"/>
        public static bool operator !=(Float3x3 left, Float3x3 right)
            => !left.Equals(right);

        /// <summary>Calculates hash code.</summary>
        /// <returns>Hash code. Consistent with overridden equality.</returns>
        public override int GetHashCode()
            => M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode()
             ^ M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode()
             ^ M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode();

        /// <summary>Formats matrix in convenient manner.</summary>
        /// <param name="format">Format string for each individual component in string representation.</param>
        /// <param name="formatProvider">Culture for formatting numbers to strings.</param>
        /// <returns>String representation of matrix in a given Culture.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => $"{M11.ToString(format, formatProvider)}\t{M12.ToString(format, formatProvider)}\t{M13.ToString(format, formatProvider)}{Environment.NewLine}" +
               $"{M21.ToString(format, formatProvider)}\t{M22.ToString(format, formatProvider)}\t{M23.ToString(format, formatProvider)}{Environment.NewLine}" +
               $"{M31.ToString(format, formatProvider)}\t{M32.ToString(format, formatProvider)}\t{M33.ToString(format, formatProvider)}{Environment.NewLine}";

        /// <summary>Formats matrix in convenient manner.</summary>
        /// <returns>String representation of matrix .</returns>
        public override string ToString()
            => $"{M11}\t{M12}\t{M13}{Environment.NewLine}" +
               $"{M21}\t{M22}\t{M23}{Environment.NewLine}" +
               $"{M31}\t{M32}\t{M33}{Environment.NewLine}";

        /// <summary>Zero matrix (all elements are 0).</summary>
        public static readonly Float3x3 Zero = new();

        /// <summary>Identity matrix (diagonal elements are 1, other are 0).</summary>
        public static readonly Float3x3 Identity = new() { M11 = 1f, M22 = 1f, M33 = 1f };
    }
}

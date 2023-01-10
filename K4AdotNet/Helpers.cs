using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace K4AdotNet
{
    /// <summary>Internal helper methods.</summary>
    internal static class Helpers
    {
        public static int UIntPtrToInt32(UIntPtr value)
            => checked((int)value.ToUInt32());

        public static UIntPtr Int32ToUIntPtr(int value)
            => new((ulong)value);

        public delegate NativeCallResults.BufferResult GetInByteBufferMethod<T>(T parameter, IntPtr buffer, ref UIntPtr size);

        public static bool TryGetValueInByteBuffer<T>(GetInByteBufferMethod<T> getMethod, T parameter,
            [NotNullWhen(returnValue: true)] out byte[]? result)
        {
            var bufferSize = UIntPtr.Zero;
            var res = getMethod(parameter, IntPtr.Zero, ref bufferSize);
            if (res == NativeCallResults.BufferResult.TooSmall)
            {
                var size = UIntPtrToInt32(bufferSize);
                if (size > 1)
                {
                    IntPtr buffer = Marshal.AllocHGlobal(size);
                    try
                    {
                        res = getMethod(parameter, buffer, ref bufferSize);
                        if (res == NativeCallResults.BufferResult.Succeeded)
                        {
                            size = UIntPtrToInt32(bufferSize);
                            result = new byte[size];
                            Marshal.Copy(buffer, result, 0, size);
                            return true;
                        }
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(buffer);
                    }
                }
            }

            if (res == NativeCallResults.BufferResult.Succeeded)
            {
                result = Array.Empty<byte>();
                return true;
            }

            result = null;
            return false;
        }

        [return: NotNullIfNotNull(parameterName: "value")]
        public static byte[]? StringToBytes(string? value, Encoding encoding)
        {
            if (value == null)
                return null;

            var byteCount = encoding.GetByteCount(value);
            var result = new byte[byteCount + 1];      // null-terminated string
            var len = encoding.GetBytes(value, 0, value.Length, result, 0);
            System.Diagnostics.Debug.Assert(len == byteCount);

            return result;
        }

        public static bool IsAsciiCompatible(string? value)
        {
            if (value == null)
                return true;
            foreach (var c in value)
                if (c > sbyte.MaxValue)
                    return false;
            return true;
        }

        public static byte[] FilePathNameToBytes(string path)
        {
            var isAsciiCompatible = IsAsciiCompatible(path);
            if (!isAsciiCompatible && Environment.OSVersion.Platform == PlatformID.Win32NT)
                throw new NotSupportedException("Non-ASCII characters in file paths are not supported in Windows version of library");
            var encoding = isAsciiCompatible ? Encoding.ASCII : Encoding.UTF8;
            return StringToBytes(path, encoding);
        }

        public static bool IsSubdirOf(DirectoryInfo subdir, DirectoryInfo parentDir)
        {
            for (; subdir != null; subdir = subdir.Parent)
                if (subdir.FullName.Equals(parentDir.FullName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            return false;
        }

        public static int IndexOf(this byte[] array, byte value)
        {
            for (var i = 0; i < array.Length; i++)
                if (array[i] == value)
                    return i;
            return -1;
        }

        public static void CheckTagName(string? tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                throw new ArgumentNullException(tagName);
            if (!IsValidTagOrTrackName(tagName))
                throw new ArgumentException($"Invalid value \"{tagName}\" of {nameof(tagName)}. Tag name must be ALL CAPS and may only contain A-Z, 0-9, '-' and '_'.", nameof(tagName));
        }

        public static void CheckTrackName(string? trackName)
        {
            if (string.IsNullOrEmpty(trackName))
                throw new ArgumentNullException(trackName);
            if (!IsValidTagOrTrackName(trackName))
                throw new ArgumentException($"Invalid value \"{trackName}\" of {nameof(trackName)}. Track name must be ALL CAPS and may only contain A-Z, 0-9, '-' and '_'.", nameof(trackName));
        }

        private static bool IsValidTagOrTrackName(string? tagName)
            => !string.IsNullOrEmpty(tagName) && tagName.All(chr => IsValidTagOrTrackNameCharacter(chr));

        private static bool IsValidTagOrTrackNameCharacter(char chr)
            => (chr >= 'A' && chr <= 'Z') || (chr >= '0' && chr <= '9') || chr == '-' || chr == '_';
    }
}

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace HapHipHop.Models
{
    public static class FingerprintConverter
    {
        public static string ConvertImageToBinary(Bitmap image)
        {
            StringBuilder binaryStringBuilder = new StringBuilder();

            int width = image.PixelSize.Width;
            int height = image.PixelSize.Height;
            int stride = width * 4;

            byte[] pixelData = new byte[height * stride];
            var pixelRect = new PixelRect(0, 0, width, height);
            var pixelFormat = PixelFormat.Bgra8888;

            GCHandle handle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
            try
            {
                IntPtr pointer = handle.AddrOfPinnedObject();
                image.CopyPixels(pixelRect, (nint)pointer, pixelData.Length, stride);
            }
            finally
            {
                handle.Free();
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    byte b = pixelData[index];
                    byte g = pixelData[index + 1];
                    byte r = pixelData[index + 2];

                    int binaryValue = (r == 0 && g == 0 && b == 0) ? 0 : 1;
                    binaryStringBuilder.Append(binaryValue);
                }
            }

            return binaryStringBuilder.ToString();
        }

        public static string ConvertBinaryToAscii(string binaryString)
        {
            StringBuilder asciiStringBuilder = new StringBuilder();

            for (int i = 0; i < binaryString.Length; i += 8)
            {
                if (i + 8 <= binaryString.Length)
                {
                    string byteString = binaryString.Substring(i, 8);
                    byte byteValue = Convert.ToByte(byteString, 2);
                    char asciiChar = (char)byteValue;
                    asciiStringBuilder.Append(asciiChar);
                }
            }

            return asciiStringBuilder.ToString();
        }

        public static string CleanPattern(string source, string pattern)
        {
            source = RemoveFirstLongestOccurrence(source, pattern);
            source = RemoveLastLongestOccurrence(source, pattern);

            int patternSize = source.Length;
            if (patternSize <= 64)
            {
                return source;
            }
            else
            {
                int middleIdx = patternSize / 2;
                int startIdx = middleIdx - 32;
                if (startIdx < 0)
                {
                    startIdx = 0;
                }
                else if (startIdx + 64 > patternSize)
                {
                    startIdx = patternSize - 64;
                }

                return source.Substring(startIdx, 64);
            }
        }

        public static string RemoveFirstLongestOccurrence(string source, string pattern)
        {
            int longestSequenceLength = 0;
            int i = 0;

            while (i <= source.Length - pattern.Length)
            {
                if (source.Substring(i, pattern.Length) == pattern)
                {
                    longestSequenceLength += pattern.Length;
                    i += pattern.Length;
                }
                else
                {
                    break;
                }
            }

            source = source.Remove(0, longestSequenceLength);
            return source;
        }

        public static string RemoveLastLongestOccurrence(string source, string pattern)
        {
            int startIndex = source.Length;
            int i = source.Length;
            while (i >= pattern.Length)
            {
                if (source.Substring(i - pattern.Length, pattern.Length) == pattern)
                {
                    startIndex = i - pattern.Length;
                    i -= pattern.Length;
                }
                else
                {
                    break;
                }
            }

            source = source.Remove(startIndex);
            return source;
        }
    }
}

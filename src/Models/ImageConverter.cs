using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Avalonia.Media.Imaging;

namespace HapHipHop.Models
{
    public static class FingerprintConverter
    {
        // Method to handle System.Drawing.Bitmap
        public static string ConvertImageToBinary(System.Drawing.Bitmap image)
        {
            return ConvertImageToBinaryInternal(image);
        }

        // Method to handle Avalonia.Media.Imaging.Bitmap
        public static string ConvertImageToBinary(Avalonia.Media.Imaging.Bitmap image)
        {
            var drawingBitmap = ConvertAvaloniaBitmapToDrawingBitmap(image);
            return ConvertImageToBinaryInternal(drawingBitmap);
        }

        private static string ConvertImageToBinaryInternal(System.Drawing.Bitmap image)
        {
            StringBuilder binaryStringBuilder = new StringBuilder();

            int width = image.Width;
            int height = image.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    int binaryValue = (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0) ? 0 : 1;
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

        // public static Bitmap CropImageWithPadding(Bitmap image, int removeWidth, int removeHeight)
        // {
        //     int width = image.Width;
        //     int height = image.Height;

        //     int newWidth = width - 2 * removeWidth;
        //     int newHeight = height - 2 * removeHeight;

        //     Bitmap croppedImage = new Bitmap(newWidth, newHeight);

        //     using (Graphics g = Graphics.FromImage(croppedImage))
        //     {
        //         g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(removeWidth, removeHeight, newWidth, newHeight), GraphicsUnit.Pixel);
        //     }

        //     return croppedImage;
        // }

        public static string CleanPattern(string source, string pattern)
        {
            source = RemoveFirstLongestOccurrence(source, pattern);
            source = RemoveLastLongestOccurence(source, pattern);

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
        
        public static string RemoveLastLongestOccurence(string source, string pattern)
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

        // Conversion methods between Avalonia Bitmap and System.Drawing.Bitmap
        public static System.Drawing.Bitmap ConvertAvaloniaBitmapToDrawingBitmap(Avalonia.Media.Imaging.Bitmap avaloniaBitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                avaloniaBitmap.Save(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new System.Drawing.Bitmap(memoryStream);
            }
        }

        public static Avalonia.Media.Imaging.Bitmap ConvertDrawingBitmapToAvaloniaBitmap(System.Drawing.Bitmap drawingBitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                drawingBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new Avalonia.Media.Imaging.Bitmap(memoryStream);
            }
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Models
{
    public static class FingerprintConverter
    {

        public static string ConvertImageToBinary(Bitmap image)
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

        public static Bitmap CropImageWithPadding(Bitmap image, int removeWidth, int removeHeight)
        {
            int width = image.Width;
            int height = image.Height;

            int newWidth = width - 2 * removeWidth;
            int newHeight = height - 2 * removeHeight;

            Bitmap croppedImage = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(removeWidth, removeHeight, newWidth, newHeight), GraphicsUnit.Pixel);
            }

            return croppedImage;
        }

        public static string CleanPattern(string source, string pattern)
        {
            source = RemoveFirstLongestOccurrence(source, pattern);
            source = RemoveLastLongestOccurence(source, pattern);
            return source;
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
    }
}
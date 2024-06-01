using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Models
{
    public static class FingerprintConverter
    {

        public static string ConvertImageToBinary(Bitmap image, int blockSize)
        {
            StringBuilder binaryStringBuilder = new StringBuilder();

            int width = image.Width;
            int height = image.Height;

            // Convert RGB to binary
            for (int y = 0; y < height; y += blockSize)
            {
                for (int x = 0; x < width; x += blockSize)
                {
                    int totalGrayValue = 0;
                    int pixelCount = 0;
                    for (int dy = 0; dy < blockSize; dy++)
                    {
                        for (int dx = 0; dx < blockSize; dx++)
                        {
                            int currentX = x + dx;
                            int currentY = y + dy;
                            if (currentX < width && currentY < height)
                            {
                                Color pixelColor = image.GetPixel(currentX, currentY);
                                int grayValue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3; // Convert to grayscale
                                totalGrayValue += grayValue;
                                pixelCount++;
                            }
                        }
                    }
                    if (pixelCount > 0)
                    {
                        int averageGrayValue = totalGrayValue / pixelCount;
                        string binaryValue = Convert.ToString(averageGrayValue, 2).PadLeft(8, '0');
                        binaryStringBuilder.Append(binaryValue);
                    }
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
    }
}
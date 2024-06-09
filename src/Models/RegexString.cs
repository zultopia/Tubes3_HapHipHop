using System;
using System.Collections.Generic;

namespace HapHipHop.Models
{
    public class RegexString
    {
        public static string ConvertAlayToOriginal(string input)
        {
            var replacements = new List<(string pattern, string replacement)>
        {
            ("1", "i"), ("1", "l"), ("2", "z"), ("3", "e"), ("4", "a"), ("5", "s"),
            ("6", "g"), ("7", "t"), ("8", "b"), ("9", "g"), ("0", "o"), ("@", "a"),
            ("!", "i"), ("$", "s"), ("&", "e"), ("#", "h")
        };

            input = ReplaceAll(input, replacements);

            input = RemoveNonAlphanumeric(input);

            var abbreviations = new List<(string pattern, string replacement)>
        {
            ("bntng", "bintang"),
            ("dw", "dwi"),
            ("mrthn", "marthen"),
        };

            input = ReplaceAll(input, abbreviations);

            input = CapitalizeProperly(input);

            return input;
        }

        static string ReplaceAll(string input, List<(string pattern, string replacement)> replacements)
        {
            foreach (var (pattern, replacement) in replacements)
            {
                input = Replace(input, pattern, replacement);
            }
            return input;
        }

        static string Replace(string input, string pattern, string replacement)
        {
            int index = input.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
            while (index != -1)
            {
                input = input.Substring(0, index) + replacement + input.Substring(index + pattern.Length);
                index = input.IndexOf(pattern, index + replacement.Length, StringComparison.OrdinalIgnoreCase);
            }
            return input;
        }

        static string RemoveNonAlphanumeric(string input)
        {
            char[] result = new char[input.Length];
            int resultIndex = 0;

            foreach (char c in input)
            {
                if (char.IsLetter(c) || char.IsWhiteSpace(c))
                {
                    result[resultIndex++] = c;
                }
            }

            return new string(result, 0, resultIndex);
        }

        static string CapitalizeProperly(string input)
        {
            string[] words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            return string.Join(" ", words);
        }

        public static string FindBestMatch(string input, List<string> names, double similarityThreshold, out double highestSimilarity)
        {
            string? bestMatch = null;
            highestSimilarity = 0.0;

            foreach (var name in names)
            {
                double similarity = CalculateSimilarity(input, name);
                if (similarity > highestSimilarity)
                {
                    highestSimilarity = similarity;
                    bestMatch = name;
                }
            }

            return highestSimilarity >= similarityThreshold ? bestMatch ?? string.Empty : "Tidak ada kecocokan yang memadai";
        }

        public static double CalculateSimilarity(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / Math.Max(source.Length, target.Length)));
        }

        static int GetMaxSimilarity(string source, string target)
        {
            int maxSimilarity = int.MaxValue;
            int sourceLength = source.Length;
            int targetLength = target.Length;

            for (int i = 0; i <= targetLength - sourceLength; i++)
            {
                string subTarget = target.Substring(i, sourceLength);
                int similarity = ComputeLevenshteinDistance(source, subTarget);
                if (similarity < maxSimilarity)
                {
                    maxSimilarity = similarity;
                }
            }

            return maxSimilarity;
        }

        static int ComputeLevenshteinDistance(string source, string target)
        {
            int n = source.Length;
            int m = target.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}

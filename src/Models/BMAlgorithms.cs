using System;
using System.Collections.Generic;

namespace HapHipHop.Models
{
  public static class BMAlgorithm
  {
    public static (List<int> positions, int comparisons, List<(int, int, char, char)> comparedCharacters) BoyerMooreSearch(string pat, string txt)
    {
      int m = pat.Length;
      int n = txt.Length;

      if (m > n)
      {
        return (new List<int>(), 0, new List<(int, int, char, char)>());
      }

      List<int> results = new List<int>();
      int comparisons = 0;
      List<(int, int, char, char)> comparedCharacters = new List<(int, int, char, char)>();

      int[] badChar = new int[256];
      BadCharHeuristic(pat, m, badChar);

      int s = 0;
      while (s <= (n - m))
      {
        int j = m - 1;

        while (j >= 0 && pat[j] == txt[s + j])
        {
          comparisons++;
          comparedCharacters.Add((s + j, j, txt[s + j], pat[j]));
          j--;
        }

        if (j < 0)
        {
          results.Add(s);
          s += (s + m < n) ? m - badChar[txt[s + m]] : 1;
        }
        else
        {
          comparisons++;
          comparedCharacters.Add((s + j, j, txt[s + j], pat[j]));
          s += Math.Max(1, j - badChar[txt[s + j]]);
        }
      }

      return (results, comparisons, comparedCharacters);
    }

    private static void BadCharHeuristic(string str, int size, int[] badChar)
    {
      for (int i = 0; i < 256; i++)
        badChar[i] = -1;

      for (int i = 0; i < size; i++)
        badChar[(int)str[i]] = i;
    }
  }
}

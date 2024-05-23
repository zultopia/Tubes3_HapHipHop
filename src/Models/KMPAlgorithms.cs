using System;
using System.Collections.Generic;

public static class KMPAlgorithm
{
    public static (List<int> positions, int comparisons, List<(int, int, char, char)> comparedCharacters) KMPSearch(string pat, string txt)
    {
        int M = pat.Length;
        int N = txt.Length;

        int[] lps = new int[M];
        int j = 0;
        int comparisons = 0; // Variable to count character comparisons
        List<(int, int, char, char)> comparedCharacters = new List<(int, int, char, char)>(); // List to store compared characters with indices

        ComputeLPSArray(pat, M, lps);

        int i = 0;
        List<int> results = new List<int>();
        while (i < N)
        {
            comparisons++; // Increment comparison count
            comparedCharacters.Add((i, j, txt[i], pat[j])); // Store compared characters with indices
            if (pat[j] == txt[i])
            {
                j++;
                i++;
            }

            if (j == M)
            {
                results.Add(i - j);
                j = lps[j - 1];
            }
            else if (i < N && pat[j] != txt[i])
            {
                if (j != 0)
                {
                    j = lps[j - 1];
                }
                else
                {
                    i++;
                }
            }
        }
        return (results, comparisons, comparedCharacters);
    }

    private static void ComputeLPSArray(string pat, int M, int[] lps)
    {
        int len = 0;
        int i = 1;
        lps[0] = 0;

        while (i < M)
        {
            if (pat[i] == pat[len])
            {
                len++;
                lps[i] = len;
                i++;
            }
            else
            {
                if (len != 0)
                {
                    len = lps[len - 1];
                }
                else
                {
                    lps[i] = len;
                    i++;
                }
            }
        }
    }
}


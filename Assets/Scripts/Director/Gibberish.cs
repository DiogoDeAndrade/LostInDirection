using System.Text;
using UnityEngine;

// The director's language. Translate turns what he means into gibberish: each word becomes a made-up word picked from
// a hash of its letters, so the same word always comes out the same and his "language" is consistent. Punctuation
// and capitals stay; text in {braces} is kept as it is (e.g. {Action!}), without the braces.
public static class Gibberish
{
    static readonly string[] syllables =
    {
        "ka", "ki", "ku", "ke", "ko", "sa", "shi", "su", "se", "so", "ta", "chi", "tsu", "te", "to",
        "na", "ni", "nu", "ne", "no", "ha", "hi", "fu", "he", "ho", "ma", "mi", "mu", "me", "mo",
        "ya", "yu", "yo", "ra", "ri", "ru", "re", "ro", "wa", "ga", "gi", "gu", "ge", "go",
        "za", "ji", "zu", "ze", "zo", "da", "de", "do", "ba", "bi", "bu", "be", "bo", "po", "n"
    };

    public static string Translate(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        var result = new StringBuilder();
        int i = 0;
        while (i < text.Length)
        {
            char c = text[i];
            if (c == '{')
            {
                int end = text.IndexOf('}', i + 1);
                if (end < 0) end = text.Length;
                result.Append(text, i + 1, end - i - 1);
                i = end + 1;
            }
            else if (char.IsLetter(c))
            {
                int start = i;
                while ((i < text.Length) && ((char.IsLetter(text[i])) || (text[i] == '\''))) i++;
                result.Append(TranslateWord(text.Substring(start, i - start)));
            }
            else
            {
                result.Append(c);
                i++;
            }
        }
        return result.ToString();
    }

    static string TranslateWord(string word)
    {
        // FNV-1a: string.GetHashCode isn't guaranteed to be the same between runs
        uint hash = 2166136261;
        foreach (char ch in word.ToLowerInvariant())
        {
            hash ^= ch;
            hash *= 16777619;
        }

        var made = new StringBuilder();
        int count = Mathf.Clamp((word.Length + 1) / 2, 1, 4);
        for (int s = 0; s < count; s++)
        {
            hash = hash * 1664525 + 1013904223;
            made.Append(syllables[(hash >> 8) % (uint)syllables.Length]);
        }

        string translated = made.ToString();
        if ((word.Length > 1) && (word.ToUpperInvariant() == word)) return translated.ToUpperInvariant();
        if (char.IsUpper(word[0])) return char.ToUpperInvariant(translated[0]) + translated.Substring(1);
        return translated;
    }
}

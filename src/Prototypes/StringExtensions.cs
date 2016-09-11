using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public static class StringExtensions {
    public static string GetLeadingWhitespace(this string s, int tabLength = 4, bool trimToLowerTab = true) {
        return new string(' ', s.GetLeadingWhitespaceLength());
    }

    public static int GetLeadingWhitespaceLength(this string s, int tabLength = 4, bool trimToLowerTab = true) {
        if (s.Length < tabLength) return 0;

        int whiteSpaceCount = 0;

        while (Char.IsWhiteSpace(s[whiteSpaceCount])) whiteSpaceCount++;

        if (whiteSpaceCount < tabLength) return 0;

        if (trimToLowerTab) {
            whiteSpaceCount -= whiteSpaceCount % tabLength;
        }

        return whiteSpaceCount;
    }
}

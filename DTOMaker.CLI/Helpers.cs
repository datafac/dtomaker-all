using System;
using System.Text;

namespace DTOMaker.CLI
{
    internal static class Helpers
    {
        public static void AppendEscaped(this StringBuilder result, ReadOnlySpan<char> input)
        {
            foreach (char ch in input)
            {
                if (ch == '"') // || ch == '\\')
                    result.Append('\\');
                result.Append(ch);
            }
        }

        public static int SizeOfLeadingWhitespace(this ReadOnlySpan<char> input)
        {
            for (int pos = 0; pos < input.Length; pos++)
            {
                if (!char.IsWhiteSpace(input[pos])) return pos;
            }
            return 0;
        }

    }
}

using System;

namespace XMAS2019.Domain.Infrastructure
{
    public static class StringExtensions
    {
        public static string MaxLengthWithDots(this string text, int maxLength)
        {
            const string dots = "...";

            int textMaxLength = maxLength - dots.Length;
            textMaxLength = Math.Max(textMaxLength, 0);

            if (text == null || text.Length <= textMaxLength)
                return text;

            return $"{text.Substring(0, textMaxLength)}{dots}";
        }

        public static string WithFirstCharToLowercase(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            return $"{char.ToLower(text[0])}{text.Substring(1)}";
        }
    }
}
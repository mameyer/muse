using System;
using System.Collections.Generic;
using System.Linq;

using TSGenerator.Models;

namespace TSGenerator.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            bool isCaps = true;

            foreach (var c in str)
            {
                if (Char.IsLetter(c) && Char.IsLower(c))
                    isCaps = false;
            }

            if (isCaps) return str.ToLower();

            return Char.ToLowerInvariant(str[0]) + str[1..];
        }

        public static string ToPascalCase(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsUpper(str, 0))
                return str;

            return Char.ToUpperInvariant(str[0]) + str[1..];
        }

        public static string ToKebabCase(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            var words = new List<string>();
            var wordStart = 0;
            int i;
            for (i = 1; i < str.Length; i++)
            {
                if (char.IsUpper(str[i]))
                {
                    words.Add(str[wordStart..i]);
                    wordStart = i;
                }
            }
            words.Add(str[wordStart..i]);

            return string.Join("-", words.Where(w => !string.IsNullOrEmpty(w)).Select(w => w.ToLower()));
        }

        public static string ToKebabCasePath(this string path)
        {
            return string.Join($"{Path.DirectorySeparatorChar}", path.Split(Path.DirectorySeparatorChar).Select(segment => ToKebabCase(segment)));
        }
    }
}

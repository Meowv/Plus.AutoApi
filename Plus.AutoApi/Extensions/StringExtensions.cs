using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Plus.AutoApi.Extensions
{
    internal static class StringExtensions
    {
        public static string RemoveSuffix(this string str, params string[] suffixes)
        {
            if (string.IsNullOrEmpty(str) || !suffixes.Any())
                return str;

            foreach (var item in suffixes)
            {
                if (str.EndsWith(item))
                {
                    return str.Left(str.Length - item.Length);
                }
            }

            return str;
        }

        public static bool IsIn(this string str, params string[] data)
        {
            return data.Any(x => x.Contains(str));
        }

        public static string Left(this string str, int len)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (str.Length < len)
                throw new ArgumentException("len argument can not be bigger than given string's length!");

            return str.Substring(0, len);
        }

        public static string GetCamelCaseFirstWord(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (str.Length == 1)
                return str;

            var res = Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})");

            if (res.Length < 1)
                return str;
            else
                return res[0];
        }

        public static string GetPascalCaseFirstWord(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (str.Length == 1)
                return str;

            var res = Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})");

            if (res.Length < 2)
                return str;
            else
                return res[1];
        }

        public static string GetPascalOrCamelCaseFirstWord(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            if (str.Length <= 1)
                return str;

            if (str[0] >= 65 && str[0] <= 90)
                return GetPascalCaseFirstWord(str);
            else
                return GetCamelCaseFirstWord(str);
        }
    }
}
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace HatenaBookmarkSharp.OAuth
{
    internal static class Utility
    {
        static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTime(this DateTime target) =>
            (long)(target - unixEpoch).TotalSeconds;

        public static string UrlEncode(this string stringToEscape) =>
            Uri.EscapeDataString(stringToEscape)
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27")
                .Replace("(", "%28")
                .Replace(")", "%29");

        public static string UrlDecode(this string stringToUnescape) =>
            UrlDecodeForPost(stringToUnescape)
                .Replace("%21", "!")
                .Replace("%2A", "*")
                .Replace("%27", "'")
                .Replace("%28", "(")
                .Replace("%29", ")");

        public static string UrlDecodeForPost(this string stringToUnescape)
        {
            stringToUnescape = stringToUnescape.Replace("+", " ");
            return Uri.UnescapeDataString(stringToUnescape);
        }


        public static IEnumerable<KeyValuePair<string, string>> ParseQueryString(
            string query,
            bool post = false)
        {
            var queryParams = query.TrimStart('?')
                .Split('&')
               .Where(x => x != "")
               .Select(x =>
               {
                   var xs = x.Split('=');
                   if (post)
                   {
                       return new KeyValuePair<string, string>(
                           xs[0].UrlDecode(),
                           xs[1].UrlDecodeForPost());
                   }
                   else
                   {
                       return new KeyValuePair<string, string>(
                           xs[0].UrlDecode(),
                           xs[1].UrlDecode());
                   }
               });
            return queryParams;
        }

        public static string Wrap(this string input, string wrapper) =>
            wrapper + input + wrapper;

        public static string ToString<T>(this IEnumerable<T> source, string separator) =>
            string.Join(separator, source);
    }
}

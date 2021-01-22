#nullable enable
using System.Collections.Generic;
using System;

namespace HatenaBookmarkSharp
{
    [Flags]
    public enum Scopes
    {
        ReadPublic = 1,
        ReadPrivate = 2,
        WritePublic = 4,
        WritePrivate = 8,

        All = ReadPublic | ReadPrivate | WritePublic | WritePrivate,
    }

    static class ScopesExtensions
    {
        public static string ToScopesString(this Scopes scopes)
        {
            var list = new List<string>();
            if (scopes.HasScope(Scopes.ReadPublic))
            {
                list.Add("read_public");
            }
            if (scopes.HasScope(Scopes.ReadPrivate))
            {
                list.Add("read_private");
            }
            if (scopes.HasScope(Scopes.WritePublic))
            {
                list.Add("write_public");
            }
            if (scopes.HasScope(Scopes.WritePrivate))
            {
                list.Add("write_private");
            }
            return string.Join(",", list);
        }

        static bool HasScope(this Scopes scopes, Scopes scope) =>
            (scopes & scope) != 0;
    }
}

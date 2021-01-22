#nullable enable
using System;

namespace HatenaBookmarkSharp.OAuth
{
    internal abstract class Token
    {
        public string Key { get; }

        public string Secret { get; }

        protected Token(string key, string secret)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Secret = secret ?? throw new ArgumentNullException(nameof(secret));
        }
    }

    internal class AccessToken : Token
    {
        public AccessToken(string key, string secret)
            : base(key, secret)
        {
        }
    }

    internal class RequestToken : Token
    {
        public RequestToken(string key, string secret)
            : base(key, secret)
        {
        }
    }
}

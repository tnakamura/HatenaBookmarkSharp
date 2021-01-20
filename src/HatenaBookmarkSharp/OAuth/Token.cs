using System;

namespace HatenaBookmarkSharp.OAuth
{
    public abstract class Token
    {
        public string Key { get; private set; }

        public string Secret { get; private set; }

        protected Token(string key, string secret)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Secret = secret ?? throw new ArgumentNullException(nameof(secret));
        }
    }

    public sealed class AccessToken : Token
    {
        public AccessToken(string key, string secret)
            : base(key, secret)
        {
        }
    }

    public sealed class RequestToken : Token
    {
        public RequestToken(string key, string secret)
            : base(key, secret)
        {
        }
    }
}

#nullable enable
using System.Linq;

namespace HatenaBookmarkSharp.OAuth
{
    internal sealed class TokenResponse<T> where T : Token
    {
        public T Token { get; }

        public ILookup<string, string> ExtraData { get; }

        public TokenResponse(T token, ILookup<string, string> extraData)
        {
            Token = token;
            ExtraData = extraData;
        }
    }
}

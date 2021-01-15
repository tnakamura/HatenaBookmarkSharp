using System;

namespace HatenaBookmarkSharp
{
    public class HatenaBookmarkException : Exception
    {
        public HatenaBookmarkException(string message)
            : base(message)
        {
        }

        public HatenaBookmarkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

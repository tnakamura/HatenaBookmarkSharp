using System;

namespace HatenaBookmarkSharp.Tests
{
    internal static class DateTimeExtensions
    {
        private static DateTime UNIX_EPOCH =
          new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long ToUnixTime(this DateTime targetTime)
        {
            targetTime = targetTime.ToUniversalTime();

            var elapsedTime = targetTime - UNIX_EPOCH;

            return (long)elapsedTime.TotalSeconds;
        }
    }
}

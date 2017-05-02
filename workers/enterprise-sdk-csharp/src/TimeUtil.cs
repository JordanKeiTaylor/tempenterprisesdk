using System;

namespace Improbable.Enterprise
{
    public class TimeUtil
    {
        private static DateTime UnixEpoch()
        {
            return new DateTime(1970, 1, 1);
        }

        public static ulong NowAsUnixTimestamp()
        {
            return (ulong) DateTime.UtcNow.Subtract(UnixEpoch()).TotalSeconds;
        }

        public static ulong NowAsUnixTimestampMillis()
        {
            return (ulong) DateTime.UtcNow.Subtract(UnixEpoch()).TotalMilliseconds;
        }

        public static DateTime UnixTimestampToDatetime(ulong timestamp)
        {
            return UnixEpoch().AddSeconds(timestamp).ToUniversalTime();
        }
    }

}


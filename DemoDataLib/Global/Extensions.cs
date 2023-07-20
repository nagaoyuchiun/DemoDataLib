using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoDataLib.Global
{
    public static class Extensions
    {
        public static long GetUnixTime(this DateTime @dateTime)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 當地時區

            return (long)(@dateTime - startTime).TotalMilliseconds;
        }
    }
}

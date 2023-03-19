using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace dotnet_markdown_sample_code
{
    internal static class TickBasedRandom
    {
        private static readonly long _startTickCount = Environment.TickCount;
        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();  // use this stopwatch to get seeds whose numbers change much faster than the tick count (which is generally every 15ms)
        private static long _rotator;      // interlocked
        public static ulong GetRandom()
        {
            return (ulong)((_startTickCount + _stopwatch.ElapsedTicks) ^ System.Threading.Interlocked.Increment(ref _rotator));   // note that, yes, the stopwatch ticks are in different units than the environment ticks they're being added to, but we don't care about that here
        }
    }
}

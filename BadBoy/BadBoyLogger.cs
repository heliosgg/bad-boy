using System;

namespace BadBoy
{
    public static class BadBoyLogger
    {
        public static Action<string> Info  { get; set; }
        public static Action<string> Error { get; set; }

        internal static void LogInfo(string log) => Info?.Invoke($"[INFO] {log}");

        internal static void LogError(string log) => Error?.Invoke($"[ERROR] {log}");
    }
}
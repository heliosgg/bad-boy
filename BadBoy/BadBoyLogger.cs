using System;

namespace BadBoy
{
    public static class BadBoyLogger
    {
        private static Action<string> _info;
        private static Action<string> _error;

        public static void RegisterInfoLogger(Action<string> logger) => _info = logger;

        public static void RegisterErrorLogger(Action<string> logger) => _error = logger;

        internal static void LogInfo(string log) => _info?.Invoke($"[INFO] {log}");

        internal static void LogError(string log) => _error?.Invoke($"[ERROR] {log}");
    }
}
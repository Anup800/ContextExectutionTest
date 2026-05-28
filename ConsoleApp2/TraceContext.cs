using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    // TraceContext.cs
    public static class TraceContext
    {
        private static readonly AsyncLocal<string> _currentId = new AsyncLocal<string>();

        // ConcurrentDictionary: lives outside AsyncLocal, thread-safe atomic increment
        private static readonly ConcurrentDictionary<string, int> _counters
            = new ConcurrentDictionary<string, int>();

        public static string CurrentId => _currentId.Value ?? string.Empty;

        public static string CreateChild()
        {
            var parentId = CurrentId;
            var index = _counters.AddOrUpdate(parentId, 1, (_, old) => old + 1);
            return string.IsNullOrEmpty(parentId) ? $"{index}" : $"{parentId}.{index}";
        }

        public static void Set(string id) => _currentId.Value = id;

        public static void Reset() => _counters.Clear(); // call between test runs
    }
}

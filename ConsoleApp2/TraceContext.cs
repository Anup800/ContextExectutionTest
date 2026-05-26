using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    public static class TraceContext
    {
        private static AsyncLocal<string> _currentId = new AsyncLocal<string>();
        private static AsyncLocal<Dictionary<string, int>> _childCounters = new AsyncLocal<Dictionary<string, int>>();

        public static string CurrentId
        {
            get => _currentId.Value ?? "1";
            set => _currentId.Value = value;
        }

        public static string CreateChild()
        {
            var parentId = CurrentId;

            if (_childCounters.Value == null)
                _childCounters.Value = new Dictionary<string, int>();

            if (!_childCounters.Value.ContainsKey(parentId))
                _childCounters.Value[parentId] = 0;

            _childCounters.Value[parentId]++;

            var childIndex = _childCounters.Value[parentId];

            return $"{parentId}.{childIndex}";
        }

        public static void Set(string id)
        {
            _currentId.Value = id;
        }
    }
}

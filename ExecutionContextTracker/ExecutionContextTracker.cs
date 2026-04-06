namespace ExecutionContextLib;

public static class ExecutionContextTracker
{
    private static AsyncLocal<string> _current = new();
    private static AsyncLocal<int> _childCounter = new();

    public static string Current => _current.Value ?? "Th.1";

    public static IDisposable StartRoot()
    {
        _current.Value = "Th.1";
        _childCounter.Value = 0;
        return new Scope(null, 0);
    }

    public static IDisposable CreateChild()
    {
        var parent = _current.Value ?? "Th.1";

        int next = ++_childCounter.Value;
        string child = $"{parent}.{next}";

        var previous = _current.Value;
        var previousCounter = _childCounter.Value;

        _current.Value = child;
        _childCounter.Value = 0;

        return new Scope(previous, previousCounter);
    }

    private class Scope : IDisposable
    {
        private readonly string _prev;
        private readonly int _prevCounter;

        public Scope(string prev, int prevCounter)
        {
            _prev = prev;
            _prevCounter = prevCounter;
        }

        public void Dispose()
        {
            _current.Value = _prev;
            _childCounter.Value = _prevCounter;
        }
    }
}
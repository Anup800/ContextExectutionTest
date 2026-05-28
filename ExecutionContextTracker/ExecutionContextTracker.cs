namespace ExecutionContextLib;

public static class ExecutionContextTracker
{
    private static readonly AsyncLocal<ExecutionState> _state = new();

    // Holds both the current ID and a thread-safe sibling counter
    private sealed class ExecutionState
    {
        public string Id { get; }

        // Counter lives on the PARENT state, not the child
        // so siblings all increment the same counter correctly
        private int _childCount;

        public ExecutionState(string id)
        {
            Id = id;
        }

        public int NextChildIndex() => Interlocked.Increment(ref _childCount);
    }

    public static string Current => _state.Value?.Id ?? "Th.1";

    public static IDisposable StartRoot()
    {
        var root = new ExecutionState("Th.1");
        var previous = _state.Value;
        _state.Value = root;
        return new Scope(previous);
    }

    public static IDisposable CreateChild()
    {
        var parentState = _state.Value;

        // If no root was started, auto-create one
        if (parentState == null)
        {
            parentState = new ExecutionState("Th.1");
        }

        // Child index comes from parent's counter — siblings share it correctly
        int index = parentState.NextChildIndex();
        string childId = $"{parentState.Id}.{index}";

        var childState = new ExecutionState(childId);
        var previous = _state.Value;
        _state.Value = childState;

        return new Scope(previous); // restore parent on Dispose
    }

    private sealed class Scope : IDisposable
    {
        private readonly ExecutionState _previous;
        private bool _disposed;

        public Scope(ExecutionState previous)
        {
            _previous = previous;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _state.Value = _previous; // restore parent state cleanly
        }
    }
}
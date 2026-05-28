using ConsoleApp2;
using MethodBoundaryAspect.Fody.Attributes;

public class LogAttribute : OnMethodBoundaryAspect
{
    public override void OnEntry(MethodExecutionArgs args)
    {
        var previousId = TraceContext.CurrentId;
        var newId = TraceContext.CreateChild();
        TraceContext.Set(newId);

        args.MethodExecutionTag = previousId; // ← save parent for restoration

        Console.WriteLine($"[ENTER] {args.Method.Name} | TraceId: {newId} | Thread: {Thread.CurrentThread.ManagedThreadId}");
    }

    public override void OnExit(MethodExecutionArgs args)
    {
        Console.WriteLine($"[EXIT]  {args.Method.Name} | TraceId: {TraceContext.CurrentId}");

        if (args.MethodExecutionTag is string prev)
            TraceContext.Set(prev);
    }

    public override void OnException(MethodExecutionArgs args)
    {
        Console.WriteLine($"[ERROR] {args.Method.Name} | TraceId: {TraceContext.CurrentId} | {args.Exception.Message}");

        if (args.MethodExecutionTag is string prev)
            TraceContext.Set(prev);
    }
}
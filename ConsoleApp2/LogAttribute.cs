using ConsoleApp2;
using MethodBoundaryAspect.Fody.Attributes;

public class LogAttribute : OnMethodBoundaryAspect
{
    public override void OnEntry(MethodExecutionArgs args)
    {
        Console.WriteLine("Again here ");
        var newId = TraceContext.CreateChild();
        TraceContext.Set(newId);

        Console.WriteLine($"[ENTER] {args.Method.Name} | TraceId: {TraceContext.CurrentId} | Thread: {Thread.CurrentThread.ManagedThreadId}");
    }

    public override void OnExit(MethodExecutionArgs args)
    {
        Console.WriteLine($"[EXIT] {args.Method.Name} | TraceId: {TraceContext.CurrentId}");
    }

    public override void OnException(MethodExecutionArgs args)
    {
        Console.WriteLine($"[EXCEPTION] {args.Method.Name} | TraceId: {TraceContext.CurrentId} | Error: {args.Exception.Message}");
    }
}
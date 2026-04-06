using ExecutionContextLib;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class ModuleWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        WriteInfo("ExecutionTracker.Fody Execute");

        foreach (var type in ModuleDefinition.Types)
        {
            foreach (var method in type.Methods)
            {
                if (!method.HasBody)
                {
                    continue;
                }

                var hasAttribute = method.CustomAttributes
                    .Any(a => a.AttributeType.Name == "TrackExecutionAttribute");

                if (!hasAttribute)
                {
                    continue;
                }

                InjectExecutionContext(method);
            }
        }
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "System";
        yield return "System.Runtime";
        yield return "ExecutionContextTracker";
    }

    private void InjectExecutionContext(MethodDefinition method)
    {
        method.Body.SimplifyMacros();

        var il = method.Body.GetILProcessor();
        var first = method.Body.Instructions.First();

        var createChildMethod = ModuleDefinition.ImportReference(
            typeof(ExecutionContextTracker).GetMethod(nameof(ExecutionContextTracker.CreateChild))
        );

        var disposeMethod = ModuleDefinition.ImportReference(
            typeof(IDisposable).GetMethod(nameof(IDisposable.Dispose))
        );

        var disposableVar = new VariableDefinition(ModuleDefinition.ImportReference(typeof(IDisposable)));
        method.Body.Variables.Add(disposableVar);
        method.Body.InitLocals = true;

        // scope = ExecutionContextTracker.CreateChild();
        il.InsertBefore(first, il.Create(OpCodes.Call, createChildMethod));
        il.InsertBefore(first, il.Create(OpCodes.Stloc, disposableVar));

        // Call Dispose before each return.
        var returns = method.Body.Instructions.Where(i => i.OpCode == OpCodes.Ret).ToList();
        foreach (var ret in returns)
        {
            il.InsertBefore(ret, il.Create(OpCodes.Ldloc, disposableVar));
            il.InsertBefore(ret, il.Create(OpCodes.Brfalse_S, ret));
            il.InsertBefore(ret, il.Create(OpCodes.Ldloc, disposableVar));
            il.InsertBefore(ret, il.Create(OpCodes.Callvirt, disposeMethod));
        }

        method.Body.OptimizeMacros();
    }
}

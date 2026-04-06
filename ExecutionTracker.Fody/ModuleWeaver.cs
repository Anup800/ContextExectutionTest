//using Common.Logging;
using ExecutionContextLib;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }

    public void Execute()
    {
        Console.WriteLine( "Fody Execute");
        foreach (var type in ModuleDefinition.Types)
        {
            foreach (var method in type.Methods)
            {
                if (!method.HasBody) continue;

                var hasAttribute = method.CustomAttributes
                    .Any(a => a.AttributeType.Name == "TrackExecutionAttribute");

                if (!hasAttribute) continue;

                InjectExecutionContext(method);
            }
        }
    }

    private void InjectExecutionContext(MethodDefinition method)
    {
        var il = method.Body.GetILProcessor();
        var first = method.Body.Instructions.First();

        // Import CreateChild method
        var createChildMethod = ModuleDefinition.ImportReference(
            typeof(ExecutionContextTracker).GetMethod("CreateChild")
        );

        var disposeMethod = ModuleDefinition.ImportReference(
            typeof(System.IDisposable).GetMethod("Dispose")
        );

        // Create variable to hold IDisposable
        var disposableVar = new VariableDefinition(
            ModuleDefinition.ImportReference(typeof(System.IDisposable))
        );
        method.Body.Variables.Add(disposableVar);

        method.Body.InitLocals = true;

        // Insert: var scope = ExecutionContextTracker.CreateChild();
        il.InsertBefore(first, il.Create(OpCodes.Call, createChildMethod));
        il.InsertBefore(first, il.Create(OpCodes.Stloc, disposableVar));

        // Wrap in try/finally (simplified version)
        var end = method.Body.Instructions.Last();

        var finallyStart = il.Create(OpCodes.Ldloc, disposableVar);
        il.Append(finallyStart);
        il.Append(il.Create(OpCodes.Callvirt, disposeMethod));

        // NOTE: full try/finally weaving is complex — this is simplified
    }
}
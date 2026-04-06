using ExecutionContextLib;

class Program
{
    static async Task Main(string[] args)
    {
        using (ExecutionContextTracker.StartRoot())
        {
            Log("Start");

            await ServiceA();

            Log("End");
        }
    }

    [TrackExecution]
    static async Task ServiceA()
    {
        Log("Inside ServiceA");
        await SubService();
        await Task.Delay(100);

        
    }

    [TrackExecution]
    static async Task SubService()
    {
        Log("Inside SubService");
        await Task.Delay(100);
    }

    static void Log(string message)
    {
        Console.WriteLine($"{ExecutionContextTracker.Current} -> {message}");
    }
}
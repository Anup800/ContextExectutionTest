using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    public class TestService
    {
        private static readonly Random _random = new Random();

        [Log]
        public async Task<int> RunSequential()
        {
            Console.WriteLine($"[SEQ START] Thread: {Thread.CurrentThread.ManagedThreadId}");

            var r1 = await Method1();
            var r2 = await Method2();
            var r3 = await Method3();
            int r4 = await RunParallel();

            Console.WriteLine($"[SEQ END] Thread: {Thread.CurrentThread.ManagedThreadId}");
            
            return r1 + r2 + r3;
        }

        [Log]
        public async Task<int> RunParallel()
        {
            Console.WriteLine($"[PARALLEL START] Thread: {Thread.CurrentThread.ManagedThreadId}");

            var tasks = new List<Task<int>>
        {
            Method1(),
            Method2(),
            Method3()
        };

            var results = await Task.WhenAll(tasks);

            Console.WriteLine($"[PARALLEL END] Thread: {Thread.CurrentThread.ManagedThreadId}");

            return results.Sum();
        }

        //[Log]
        //public async Task<int> RunWithException()
        //{
        //   Console.WriteLine($"[EXCEPTION TEST START] Thread: {Thread.CurrentThread.ManagedThreadId}");

        //    var tasks = new List<Task<int>>
        //{
        //    Method1(),
        //    MethodWithException(), // 💥 this will throw
        //    Method3()
        //};

        //    var results = await Task.WhenAll(tasks); // will trigger OnException

        //    return results.Sum();
        //}

        [Log]
        public async Task<int> Method1()
        {
            await SimulateWork("Method1");
            return 1;
        }

        [Log]
        public async Task<int> Method2()
        {
            await SimulateWork("Method2");
            return 2;
        }

        [Log]
        public async Task<int> Method3()
        {
            await SimulateWork("Method3");
            return 3;
        }

        //[Log]
        //public async Task<int> MethodWithException()
        //{
        //    await SimulateWork("MethodWithException");
        //    throw new Exception("Something went wrong!");
        //}

        private async Task<int> SimulateWork(string methodName)
        {
            try
            {
                
                Task.Delay(1000);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return 0;
            }

           // Console.WriteLine(
              //  $"{methodName} END   - Thread: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}

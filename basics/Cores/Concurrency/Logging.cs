using System;
using System.IO;
using System.Threading;

namespace Cores.Concurrency;

public class Logging
{
    private static Mutex mutex = new Mutex();

    static void WriteLog(string threadName)
    {
        Console.WriteLine($"{threadName} is waiting...");
        mutex.WaitOne(); // Acquire the mutex
        try
        {
            Console.WriteLine($"{threadName} entered critical section.");
            using (StreamWriter sw = new StreamWriter("c:\\kvr\\log.txt", true))
            {
                sw.WriteLine($"{DateTime.Now}: {threadName} wrote to log.");
            }
            Thread.Sleep(2000); // Simulate work
            Console.WriteLine($"{threadName} leaving critical section.");
        }
        finally
        {
            mutex.ReleaseMutex(); // Release the mutex
        }
    }

    public static void Main()
    {
        Thread t1 = new Thread(() => WriteLog("Thread 1"));
        Thread t2 = new Thread(() => WriteLog("Thread 2"));
        Thread t3 = new Thread(() => WriteLog("Thread 3"));

        t1.Start();
        t2.Start();
        t3.Start();

        t1.Join();
        t2.Join();
        t3.Join();

        Console.WriteLine("All threads finished.");
    }
}


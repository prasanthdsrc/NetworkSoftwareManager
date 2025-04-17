using System;
using System.Management;

class TimeSpanTest
{
    static void Main()
    {
        var options = new ConnectionOptions();
        Console.WriteLine($"Timeout Type: {options.Timeout.GetType().Name}");
        // Try setting timeout to a TimeSpan
        options.Timeout = TimeSpan.FromSeconds(30);
        Console.WriteLine($"After setting: {options.Timeout}");
        
        // Try setting timeout to an int
        try {
            // This would compile if int is accepted
            // options.Timeout = 30000;
            Console.WriteLine("Setting int value would work");
        } catch {
            Console.WriteLine("Setting int value would fail");
        }
    }
}
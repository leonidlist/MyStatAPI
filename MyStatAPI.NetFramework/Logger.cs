using System;

namespace MyStatAPI.Full
{
    public static class Logger
    {
        public static void Log(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {message}");
            Console.ResetColor();
        }
    }
}
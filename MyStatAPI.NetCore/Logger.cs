using System;
using System.Collections.Generic;
using System.Text;

namespace MyStatAPI
{
    public static class Logger
    {
        public static void Log(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

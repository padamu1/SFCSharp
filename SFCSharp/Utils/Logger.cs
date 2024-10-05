using System;

namespace SFCSharp.Utils
{
    public class Logger
    {
        public static Logger? Instance { get; private set; }

        public Logger()
        {
            Instance = new Logger();
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void Error(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}

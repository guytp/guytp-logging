using Guytp.Logging;
using System;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger logger = Logger.ApplicationInstance;
            logger.Debug("Debug test");
            logger.Info("Info test");
            logger.Warn("Warn test");
            logger.Error("Error test");
            DateTime start = DateTime.UtcNow;
            for (int i = 0; i < 10000; i++)
                logger.Info("Logging " + i);
            DateTime end = DateTime.UtcNow;
            logger.Dispose();
            Console.Write("All logging tests performed in " + end.Subtract(start).TotalMilliseconds + "ms, press any key to exit...");
            Console.ReadKey();
        }
    }
}
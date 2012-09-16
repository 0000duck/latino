using System;
using Latino;

namespace Latino.Tutorials
{
    class Program
    {
        static void Main(string[] args)
        {
            // create two loggers
            Logger logger1 = Logger.GetLogger("Latino.Tutorials.Example9.Logger1");
            Logger logger2 = Logger.GetLogger("Latino.Tutorials.Example9.Logger2");
            // output a message and a warning
            logger1.Info("Main", "This message is brought to you by Logger 1.");
            logger2.Warn("Main", "This warning is brought to you by Logger 2.");
            // change the output format of Logger 2
            logger2.CustomOutput = new Logger.CustomOutputDelegate(
                delegate(string loggerName, Logger.Level level, string funcName, Exception exception, string message, object[] msgArgs)
                {
                    Console.WriteLine("{0} says: \"{1}\"", loggerName, string.Format(message, msgArgs));
                });
            logger2.LocalOutputType = Logger.OutputType.Custom;
            // output the message and warning again
            logger1.Info("Main", "This message is brought to you by Logger 1.");
            logger2.Warn("Main", "This warning is brought to you by Logger 2.");
            // set both loggers to output only warnings, errors, and fatal errors
            Logger.GetRootLogger().LocalLevel = Logger.Level.Warn;
            logger1.Trace("Main", "This trace message is brought to you by Logger 1."); // this will not be displayed
            logger1.Debug("Main", "This debug message is brought to you by Logger 1."); // this will not be displayed
            logger2.Info("Main", "This message is brought to you by Logger 2."); // this will not be displayed
            logger2.Warn("Main", "This warning is brought to you by Logger 2.");
        }
    }
}
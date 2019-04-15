using System;

namespace CodeReview
{
    class Program
    {
        static void Main(string[] args)
        {
            var jobLogger = new JobLogger(new JobLoggerOptions  
            {
                Destinations = new LogDestinationEnum[] { LogDestinationEnum.Console, LogDestinationEnum.File },
                Levels = new LogLevelEnum[] { LogLevelEnum.Message, LogLevelEnum.Warning }
            });

            jobLogger.LogMessage("demo");
            jobLogger.LogWarning("hi!");
        }
    }
}

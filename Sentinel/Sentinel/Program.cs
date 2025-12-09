using Sentinel.Models;
using Sentinel.Services.LoggerBuilders;

namespace Sentinel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var builder = LoggerBuilder.CreateLogger();

            var builder = LoggerBuilder.CreateLogger(options =>
            {
                options
                    //.AddConsoleLogger()
                    //.AddJsonLogger()
                    .AddJsonLogger(options =>
                    {
                        options
                        .WithLogFilePath("D:\\Logs")
                        .WithLoggedClassFilter("Program")
                        .WithLogFileName("Test")
                        .WithMinimumLogLevel(LogLevel.WARNG)
                        .WithSinkRollTiming(SinkRoll.DAILY);
                    });
                    //.AddXmlLogger()
                    //.AddXmlLogger()
                    //.AddCustomLogger<TxtLogWriter>();
            });

            var logger = builder.GetLogger<Program>();

            logger.Log(Models.LogLevel.VERBS, "Hello 1");
            logger.LogDebug("Hello 2");
            logger.LogInformation("Hello 3");
            logger.LogWarning("Hello 4");
            logger.LogError("Hello 5");
            logger.LogFatal("Hello 6");

            //while (true)
            //{

            //    logger.Log(Models.LogLevel.VERBS, "Hello 1");
            //    logger.LogDebug("Hello 2");
            //    logger.LogInformation("Hello 3");
            //    logger.LogWarning("Hello 4");
            //    logger.LogError("Hello 5");
            //    logger.LogFatal("Hello 6");
            //    Task.Delay(TimeSpan.FromSeconds(3)).Wait();
            //}

            logger.ShutDown();
        }
    }
}

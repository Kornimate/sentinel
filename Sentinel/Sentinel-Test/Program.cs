using Sentinel.Models;
using Sentinel.Services.LoggerBuilders;

namespace Sentinel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = LoggerBuilder.CreateLogger(options =>
            {
                options
                    .AddConsoleLogger()
                    .AddJsonLogger(options =>
                    {
                        options
                        .WithLogFilePath("D:\\Logs")
                        .WithLoggedClassFilter("Program")
                        .WithLogFileName("Test")
                        .WithMinimumLogLevel(LogLevel.WARNG)
                        .WithSinkRollTiming(SinkRoll.DAILY);
                    })
                    .AddXmlLogger();
            });

            var logger = builder.GetLogger<Program>();

            logger.ShutDown();
        }
    }
}

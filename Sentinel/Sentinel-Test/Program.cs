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
                    .AddConsoleLogger(options =>
                    {
                        options.WithLogContainerSize(5000);
                    })
                    .AddJsonLogger(options =>
                    {
                        options
                        .WithLogFileName("Test")
                        .WithLogFilePath("D:\\Logs")
                        .WithSinkRollTiming(SinkRoll.DAILY)
                        .WithMinimumLogLevel(LogLevel.WARNG)
                        .WithLoggedClassFilter("Program");
                    })
                    .AddXmlLogger();
            });

            var logger = builder.GetLogger<Program>();

            logger.ShutDown();
        }
    }
}

using Sentinel.Services.LoggerBuilders;

namespace Sentinel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = LoggerBuilder.CreateLogger(options =>
            {
                options.AddConsoleLogger()
                       .AddJsonLogger();
            });

            var logger = builder.GetLogger<Program>();

            logger.Log(Models.LogLevel.VERBS, "Hello 1");
            logger.LogDebug("Hello 2");
            logger.LogInformation("Hello 3");
            logger.LogWarning("Hello 4");
            logger.LogError("Hello 5");
            logger.LogFatal("Hello 6");

            logger.ShutDown();
        }
    }
}

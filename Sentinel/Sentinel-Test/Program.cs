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
                    .AddJsonLogger()
                    .AddXmlLogger();
            });

            var logger = builder.GetLogger<Program>();

            for(int i=0; i< 1_000_000; i++)
            {
                logger.LogInformation("Info");
            }

            logger.ShutDown();
        }
    }
}

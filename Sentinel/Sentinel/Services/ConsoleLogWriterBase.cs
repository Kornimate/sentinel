using Sentinel.Models.Interfaces;
using Sentinel.Models;
using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services
{
    public abstract class ConsoleLogWriterBase : LogWriterBase
    {
        public ConsoleLogWriterBase() : base() { }

        public override bool WriteToConsole() => true;

        public sealed override ILogWriter Build()
        {
            return base.Build();
        }

        protected sealed override async Task ConsumeAsync()
        {
            await foreach (var entry in _channel?.Reader.ReadAllAsync()!)
            {
                await WriteLogAsync(entry);
            }
        }

        protected override Task WriteLogAsync(ILogEntry entry)
        {
            Console.WriteLine(entry.Serialize());

            return Task.CompletedTask;
        }
    }
}

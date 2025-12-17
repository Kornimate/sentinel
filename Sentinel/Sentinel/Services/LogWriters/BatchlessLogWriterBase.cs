using Sentinel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentinel.Services.LogWriters.Interfaces;
using Sentinel.Models.LogTypes.Interfaces;

namespace Sentinel.Services.LogWriters
{
    public abstract class BatchlessLogWriterBase : LogWriterBase
    {
        public BatchlessLogWriterBase() : base() { }

        protected sealed override async Task ConsumeAsync(CancellationToken token)
        {
            await foreach (var entry in _channel?.Reader.ReadAllAsync(token)!)
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

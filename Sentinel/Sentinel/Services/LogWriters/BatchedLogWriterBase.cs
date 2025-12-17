using Sentinel.Models;
using Sentinel.Models.LogTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sentinel.Services.LogWriters
{
    public class BatchedLogWriterBase : LogWriterBase
    {
        protected readonly IList<ILogEntry> _batch = [];

        protected override async Task ConsumeAsync(CancellationToken token)
        {
            try
            {
                await foreach (var entry in _channel!.Reader.ReadAllAsync(token))
                {
                    await WriteEntryToBatchOrFlush(entry);
                }
            }
            finally
            {
                if (_batch.Count > 0)
                    await FlushBatchAsync();
            }
        }

        protected virtual async Task WriteEntryToBatchOrFlush(ILogEntry entry)
        {
            _batch.Add(entry);

            if (entry.Level >= LogLevel.ERROR || _batch.Count >= BatchSize)
            {
                await FlushBatchAsync();
            }
        }

        protected virtual async Task FlushBatchAsync()
        {

            foreach (var entry in _batch)
            {
                await WriteLogAsync(entry);
            }

            _batch.Clear();
        }

        protected override Task WriteLogAsync(ILogEntry entry)
        {
            Console.WriteLine(entry.Serialize());

            return Task.CompletedTask;
        }

        public override async ValueTask DisposeAsync()
        {
            if (_batch.Count > 0)
                await FlushBatchAsync();

            await base.DisposeAsync();
        }
    }
}

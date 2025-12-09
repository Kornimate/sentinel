using Sentinel.Models.Interfaces;
using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sentinel.Services
{
    internal sealed class JsonLogWriter : FileLogWriterBase
    {
        public JsonLogWriter() : base()
        {
            FileExtension = ".json";
            SubDirectory = "Json";
        }

        protected override async Task WriteLogAsync(ILogEntry entry)
        {
            await _writer.WriteLineAsync(JsonSerializer.Serialize(new
            {
                level = entry.Level.ToString(),
                ts = entry.TimeStamp.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                message = entry.Text,
                exception = entry.Exception is null ? "" : entry.Exception.ToString(),
            }));
        }
    }
}

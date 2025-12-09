using Sentinel.Models;
using Sentinel.Models.Interfaces;
using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sentinel.Services
{
    public abstract class LogWriterBase : ILogWriter, IAsyncDisposable
    {
        // ----- Configs ------
        protected string? _filter = null;
        protected LogLevel _minimumLevel = LogLevel.VERBOSE;
        // ------ Configs end ------

        protected static readonly int _channelSize = 50_000;
        protected readonly static int BatchSize = 5;

        protected Channel<ILogEntry>? _channel;
        private Task? _backgroundTask;

        protected LogWriterBase() { }

        public Task StartNewBackgroundTask()
        {
            if (_backgroundTask != null && !_backgroundTask.IsCompleted)
                return _backgroundTask;

            _backgroundTask = Task.Run(ConsumeAsync);

            return _backgroundTask;
        }

        public Task? GetBackgroundConsumerTask()
        {
            return _backgroundTask;
        }

        public virtual ILogWriter Build()
        {
            _channel = Channel.CreateBounded<ILogEntry>(new BoundedChannelOptions(_channelSize)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

            _backgroundTask = StartNewBackgroundTask();

            return this;
        }

        public virtual void AddLogMessage(object? sender, ILogEntry log)
        {
            if (log is null) // only valid logs
                return;

            if (log.Level < _minimumLevel) // filter logs with LogLevel smaller than specified
                return;

            if (_filter != null && log.FilterData.IndexOf(_filter) == -1) // if filter is set filter for class names (full name with namespace)
                return;

            bool validChannel = _channel is not null;

            if (validChannel && _channel!.Writer.TryWrite(log)) // try sync write
                return;

            if (validChannel)
            {
                _ = _channel!.Writer.WriteAsync(log); // if not start async write process
            }
        }

        public virtual async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            _channel?.Writer.Complete();

            if (_backgroundTask != null)
            {
                await _backgroundTask!;
            }
        }
        public void ShutDown()
        {
            DisposeAsync().AsTask().Wait();
        }

        protected abstract Task ConsumeAsync();

        protected abstract Task WriteLogAsync(ILogEntry entry);

        public virtual void SetFilePath(string filePath) => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual void SetFileName(string fileName) => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual void SetFilter(string filter) => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual void SetMinimiumLogLevel(LogLevel logLevel) => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual void SetSinkTiming(SinkRoll sinkRoll) => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual void SetSubDirectory(string dirName) => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual string? GetFilePath() => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual string? GetFileName() => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual string? GetSubDirectory() => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual bool WriteToConsole() => false;
    }
}

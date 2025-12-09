using Sentinel.Models;
using Sentinel.Models.LogTypes.Interfaces;
using Sentinel.Services.LogWriters.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sentinel.Services.LogWriters
{
    public abstract class LogWriterBase : ILogWriter, IAsyncDisposable
    {
        // ----- Configs ------
        protected string? _filter = null;
        protected LogLevel _minimumLevel = LogLevel.VERBS;
        // ------ Configs end ------

        protected static readonly int _channelSize = 50_000;
        protected readonly static int BatchSize = 5;

        protected Channel<ILogEntry>? _channel;
        private Task? _backgroundTask;
        protected CancellationTokenSource _cts;

        private bool _alreadyBuilt;
        private bool _alreadyDisposed;

        protected LogWriterBase()
        {
            _cts = new CancellationTokenSource();
            _alreadyBuilt = false;
            _alreadyDisposed = false;
        }

        public Task StartNewBackgroundTask()
        {
            if (_backgroundTask != null && !_backgroundTask.IsCompleted)
                return _backgroundTask;

            _backgroundTask = Task.Run(() => ConsumeAsync(_cts.Token));

            return _backgroundTask;
        }

        public Task? GetBackgroundConsumerTask()
        {
            return _backgroundTask;
        }

        public virtual ILogWriter Build()
        {
            if (_alreadyBuilt)
                return this;

            _channel = Channel.CreateBounded<ILogEntry>(new BoundedChannelOptions(_channelSize)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

            _backgroundTask = StartNewBackgroundTask();

            _alreadyBuilt = true;

            AppDomain.CurrentDomain.ProcessExit += (_, _) => ShutDown();
            Console.CancelKeyPress += (_, _) => ShutDown();

            return this;
        }

        public virtual void AddLogMessage(object? sender, ILogEntry log)
        {
            if (log is null) // only valid logs
                return;

            if (log.Level < _minimumLevel) // filter logs with LogLevel smaller than specified
                return;

            if (EntryPassesFilter(log)) // if filter is set filter for class names (full name with namespace)
                return;

            bool validChannel = _channel is not null;

            try
            {
                if (validChannel && _channel!.Writer.TryWrite(log)) // try sync write
                    return;

                if (validChannel)
                {
                    _ = _channel!.Writer.WriteAsync(log); // if not start async write process
                }
            }
            catch (ChannelClosedException) { } // dont to anything cause the channel is closed
        }

        public virtual async ValueTask DisposeAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5)); // give time to empty the channels 

            _cts?.Cancel();

            _channel?.Writer.TryComplete();

            try
            {
                if (_backgroundTask != null)
                    await _backgroundTask;
            }
            catch (OperationCanceledException) { } // swallow it

            GC.SuppressFinalize(this);
        }

        public void ShutDown()
        {
            if (_alreadyDisposed)
                return;

            DisposeAsync().AsTask().Wait();

            _alreadyDisposed = true;
        }

        protected abstract Task ConsumeAsync(CancellationToken token);

        protected abstract Task WriteLogAsync(ILogEntry entry);

        protected virtual bool EntryPassesFilter(ILogEntry log)
        {
            return _filter != null && log.FilterData.IndexOf(_filter) == -1; // fast check
        }

        public void SetFilter(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                throw new ArgumentException("Invalid filter!", nameof(filter));

            _filter = filter;
        }

        public void SetMinimiumLogLevel(LogLevel logLevel)
        {
            _minimumLevel = logLevel;
        }

        public virtual void SetFilePath(string filePath) => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual void SetFileName(string fileName) => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");


        public virtual void SetSinkTiming(SinkRoll sinkRoll) => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual void SetSubDirectory(string dirName) => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual string? GetFilePath() => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual string? GetFileName() => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual string? GetSubDirectory() => throw new NotImplementedException("This method is only implemented in FileLogWriterBase and derived types!");

        public virtual bool WriteToConsole() => false;
    }
}

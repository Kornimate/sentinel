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
    public class LogWriterBase : ILogWriter, IAsyncDisposable
    {
        // ----- Configs ------
        protected string? _filePath = null;
        protected string? _subDirectory = null;
        protected string? _fileName = null;
        protected string? _filter = null;
        protected LogLevel _minimumLevel = LogLevel.VERBOSE;
        protected SinkRoll _sinkTiming = SinkRoll.DAILY;
        // ------ Configs end ------

        private const int CHANNEL_SIZE = 50_000;
        protected Channel<ILogEntry> _channel;

        private Task _backgroundTask;
        private Task _flushDelay;

        protected FileStream _stream = default!;
        protected StreamWriter _writer = default!;

        protected readonly TimeSpan _flushInterval;
        protected readonly TimeSpan _rotationInterval;

        protected readonly static int BatchSize = 5;

        protected readonly IList<ILogEntry> _batch = [];

        protected DateTime _nextRotationTime;

        private bool WritingToConsole { get; set; }

        protected LogWriterBase(string? subDirectory)
        {
            _subDirectory = subDirectory;
            if (_subDirectory != null) // if subdirectory is null, log to the console
            {
                WritingToConsole = false;
                _filePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "Logs", _subDirectory);
            }

            _flushInterval = TimeSpan.FromSeconds(1);
            _rotationInterval = TimeSpan.FromHours((int)_sinkTiming);
            _flushDelay = Task.CompletedTask;

            _channel = Channel.CreateBounded<ILogEntry>(new BoundedChannelOptions(CHANNEL_SIZE)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            });

            OpenNewOrExistingFile();

            _backgroundTask = StartNewBackgroundTask();
        }

        public Task StartNewBackgroundTask()
        {
            if (_backgroundTask != null && !_backgroundTask.IsCompleted)
                return _backgroundTask;

            _backgroundTask = Task.Run(ConsumeAsync);

            return _backgroundTask;
        }

        public virtual async Task ConsumeAsync()
        {
            _flushDelay = Task.Delay(_flushInterval);

            try
            {
                await foreach (var entry in _channel.Reader.ReadAllAsync())
                {
                    await WriteLogAsync(entry);
                }
            }
            finally
            {
                if (_batch != null && _batch.Count > 0)
                    await FlushBatchAsync(forceFsync: true);

                await CloseCurrentFileAsync();
            }
        }

        protected virtual async Task WriteLogAsync(ILogEntry entry)
        {
            _batch.Add(entry);

            if (entry.Level >= LogLevel.ERROR)
            {
                await FlushBatchAsync(forceFsync: true);
                _flushDelay = Task.Delay(_flushInterval);
            }


            else if (_batch.Count >= BatchSize)
            {
                await FlushBatchAsync(forceFsync: false);
                _flushDelay = Task.Delay(_flushInterval);
            }

            if (_flushDelay.IsCompleted)
            {
                await FlushBatchAsync(forceFsync: false);
                _flushDelay = Task.Delay(_flushInterval);
            }

            if (ShouldRotate())
            {
                await RotateAsync();
            }
        }

        protected virtual bool ShouldRotate()
        {
            if (DateTime.UtcNow >= _nextRotationTime)
                return true;

            return false;
        }

        protected virtual async Task RotateAsync()
        {
            if (WritingToConsole)
                return;

            await FlushBatchAsync(forceFsync: true);
            await CloseCurrentFileAsync();
            OpenNewOrExistingFile();
        }

        protected virtual void OpenNewOrExistingFile()
        {
            if (WritingToConsole)
                return;

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HH");
            var path = Path.Combine(_filePath, $"{timestamp}{(_fileName is not null ? "_" + _fileName : ".log")}");

            _stream = new FileStream(
                path,
                FileMode.OpenOrCreate,
                FileAccess.Write,
                FileShare.Read,
                bufferSize: 64 * 1024,
                options: FileOptions.Asynchronous | FileOptions.WriteThrough);

            _writer = new StreamWriter(_stream) { AutoFlush = false };

            _nextRotationTime = DateTime.UtcNow + _rotationInterval;
        }

        protected virtual async Task CloseCurrentFileAsync()
        {
            if (WritingToConsole)
                return;

                await _writer.FlushAsync();
            _stream.Flush(true);

            await _writer.DisposeAsync();
            _stream.Dispose();
        }

        protected virtual async Task FlushBatchAsync(bool forceFsync)
        {
            if (WritingToConsole)
                return; 

            foreach (var entry in _batch)
            {
                await WriteEntryToFile(entry);
            }

            _batch.Clear();

            await _writer.FlushAsync();

            if (forceFsync)
            {
                _stream.Flush(true);
            }
        }

        protected virtual Task WriteEntryToFile(ILogEntry entry)
        {
            return _writer.WriteLineAsync(entry.Serialize());
        }

        public virtual void AddLogMessage(object? sender, ILogEntry log)
        {
            if (log is null)
                return;

            if (_filter != null && log.FilterData.IndexOf(_filter) == -1)
                return;

            if (_channel.Writer.TryWrite(log)) // try sync write
                return;

            _ = _channel.Writer.WriteAsync(log); // if not start async write process
        }

        public virtual void SetFilePath(string filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid file path!", nameof(filePath));

            if (!Path.Exists(filePath))
                throw new ArgumentException("Path does not exist!");

            _filePath = filePath;
        }

        public virtual void SetFileName(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Invalid file name!", nameof(fileName));

            if (fileName.IndexOf(".") == -1)
                throw new ArgumentException("File name has to contain extension too!", nameof(fileName));

            _fileName = fileName;
        }

        public virtual void SetFilter(string filter)
        {
            if (String.IsNullOrWhiteSpace(filter))
                throw new ArgumentException("Invalid filter!", nameof(filter));

            throw new NotImplementedException();
        }

        public virtual void SetMinimiumLogLevel(LogLevel logLevel)
        {
            _minimumLevel = logLevel;
        }

        public virtual void SetSinkTiming(SinkRoll sinkRoll)
        {
            _sinkTiming = sinkRoll;
        }
        public Task? GetBackgroundConsumerTask()
        {
            return _backgroundTask;
        }

        public virtual void SetSubDirectory(string dirName)
        {
            if (String.IsNullOrWhiteSpace(dirName))
                throw new ArgumentException("Invalid file path!", nameof(dirName));

            _subDirectory = dirName;
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            _channel.Writer.Complete();
            await _backgroundTask;
        }
    }
}

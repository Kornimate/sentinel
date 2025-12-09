using Sentinel.Models;
using Sentinel.Models.Interfaces;
using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services
{
    public class FileLogWriterBase : LogWriterBase
    {
        // ----- Configs ------
        protected string? _filePath = null;
        protected string? _fileName = null;
        protected SinkRoll _sinkTiming = SinkRoll.DAILY;
        // ----- Configs end -----

        protected FileStream _stream = default!;
        protected StreamWriter _writer = default!;

        private Task? _flushDelay;
        protected readonly TimeSpan _flushInterval;
        protected readonly TimeSpan _rotationInterval;
        protected DateTime _nextRotationTime;

        protected readonly IList<ILogEntry> _batch = [];

        protected string SubDirectory { get; set; } = "Default";
        public string FileExtension { get; set; } = ".log";

        protected FileLogWriterBase() : base()
        {
            _flushInterval = TimeSpan.FromSeconds(1);
            _rotationInterval = TimeSpan.FromHours((int)_sinkTiming);
            _flushDelay = Task.CompletedTask;
        }

        public sealed override ILogWriter Build()
        {
            if (SubDirectory == null)
                throw new InvalidProgramException("Subdirectory is not set on log writer!");

            if (_filePath is null)
            {
                _filePath = Path.Combine(Assembly.GetExecutingAssembly().Location, "Logs", SubDirectory);
            }
            else
            {
                _filePath = Path.Combine(_filePath, SubDirectory);
            }

            OpenNewOrExistingFile(); // open file before background task starts to write it (starts in base.Build())

            return base.Build();
        }

        protected virtual void OpenNewOrExistingFile()
        {

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HH");
            var path = Path.Combine(_filePath!, $"{timestamp}{(_fileName is not null ? "_" + _fileName : "")}{(FileExtension.IndexOf(".") == -1 ? "." + FileExtension : FileExtension)}");

            if (_writer != default && _writer != null)
            {
                _writer?.Dispose();
            }

            if (_stream != default && _stream != null)
            {
                _stream?.Dispose();
            }

            _stream = new FileStream(
                path,
                FileMode.Append,
                FileAccess.Write,
                FileShare.Read,
                bufferSize: 64 * 1024,
                options: FileOptions.Asynchronous | FileOptions.WriteThrough);


            _writer = new StreamWriter(_stream) { AutoFlush = false };

            _nextRotationTime = DateTime.UtcNow + _rotationInterval;
        }

        protected override async Task ConsumeAsync()
        {
            _flushDelay = Task.Delay(_flushInterval);

            try
            {
                await foreach (var entry in _channel?.Reader.ReadAllAsync()!)
                {
                    await WriteEntryToFileOrBatch(entry);
                }
            }
            finally
            {
                if (_batch != null && _batch.Count > 0)
                    await FlushBatchAsync(forceFsync: true);

                await CloseCurrentFileAsync();
            }
        }

        protected override Task WriteLogAsync(ILogEntry entry)
        {
            return _writer.WriteLineAsync(entry.Serialize());
        }

        protected virtual async Task WriteEntryToFileOrBatch(ILogEntry entry)
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

            if (_flushDelay is not null && _flushDelay.IsCompleted)
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
            await FlushBatchAsync(forceFsync: true);
            await CloseCurrentFileAsync();
            OpenNewOrExistingFile();
        }

        protected virtual async Task CloseCurrentFileAsync()
        {
            await _writer.FlushAsync();
            _stream.Flush(true);

            await _writer.DisposeAsync();
            _stream.Dispose();
        }

        protected virtual async Task FlushBatchAsync(bool forceFsync)
        {
            foreach (var entry in _batch)
            {
                await WriteLogAsync(entry);
            }

            _batch.Clear();

            await _writer.FlushAsync();

            if (forceFsync)
            {
                _stream.Flush(true);
            }
        }

        public sealed override void SetFilePath(string filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid file path!", nameof(filePath));

            if (!Path.Exists(filePath))
                throw new ArgumentException("Path does not exist!");

            _filePath = filePath;
        }

        public sealed override void SetFileName(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Invalid file name!", nameof(fileName));

            _fileName = fileName;
        }

        public sealed override void SetFilter(string filter)
        {
            if (String.IsNullOrWhiteSpace(filter))
                throw new ArgumentException("Invalid filter!", nameof(filter));

            throw new NotImplementedException();
        }

        public sealed override void SetMinimiumLogLevel(LogLevel logLevel)
        {
            _minimumLevel = logLevel;
        }

        public sealed override void SetSinkTiming(SinkRoll sinkRoll)
        {
            _sinkTiming = sinkRoll;
        }
        public sealed override void SetSubDirectory(string dirName)
        {
            if (String.IsNullOrWhiteSpace(dirName))
                throw new ArgumentException("Invalid file path!", nameof(dirName));

            SubDirectory = dirName;
        }

        public override async ValueTask DisposeAsync()
        {
            _channel?.Writer.Complete();

            await base.DisposeAsync();
        }

        public sealed override string? GetFilePath()
        {
            return _filePath;
        }

        public sealed override string? GetFileName()
        {
            return _fileName;
        }

        public sealed override string? GetSubDirectory()
        {
            return SubDirectory;
        }
    }
}

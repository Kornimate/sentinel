using Sentinel.Models;
using Sentinel.Models.LogTypes.Interfaces;
using Sentinel.Services.LogWriters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.LogWriters
{
    public abstract class FileLogWriterBase : BatchedLogWriterBase, IFileLogWriter
    {
        private static int _loggerIndexer = 0;

        // ----- Config fields ------
        protected string? _filePath = null;
        protected string? _fileName = null;
        protected SinkRoll _sinkTiming = SinkRoll.DAILY;

        protected FileStream _stream = default!;
        protected StreamWriter _writer = default!;

        private Task? _flushDelay;
        protected readonly TimeSpan _flushInterval;
        protected TimeSpan _rotationInterval;
        protected DateTime _nextRotationTime;

        protected readonly IList<ILogEntry> _batch = [];

        protected string SubDirectory { get; set; } = "Default";
        public string FileExtension { get; set; } = ".log";

        private bool _isClosed; // double-dispose guard

        private readonly SemaphoreSlim _ioLock; // guard for stream

        protected FileLogWriterBase() : base()
        {
            _flushInterval = TimeSpan.FromSeconds(1);
            _rotationInterval = TimeSpan.FromHours((int)_sinkTiming);
            _flushDelay = Task.CompletedTask;
            _ioLock = new SemaphoreSlim(1, 1);
        }

        public sealed override ILogWriter Build()
        {
            if (SubDirectory == null)
                throw new InvalidProgramException("Subdirectory is not set on log writer!");

            if (_filePath is null)
            {
                _filePath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                    "Logs",
                    SubDirectory);
            }
            else
            {
                _filePath = Path.Combine(_filePath, SubDirectory);
            }

            _fileName ??= (++_loggerIndexer).ToString();

            _rotationInterval = TimeSpan.FromHours((int)_sinkTiming);

            OpenNewOrExistingFile();

            return base.Build();
        }

        protected override async Task ConsumeAsync(CancellationToken token)
        {
            _flushDelay = Task.Delay(_flushInterval); // NO token should be passed

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
                    await FlushBatchAsync(forceFsync: true);

                await CloseCurrentFileAsync();
            }
        }

        protected override Task WriteLogAsync(ILogEntry entry)
        {
            return _writer.WriteLineAsync(entry.Serialize());
        }

        protected override async Task WriteEntryToBatchOrFlush(ILogEntry entry)
        {
            _batch.Add(entry);

            if (entry.Level >= LogLevel.ERROR || _batch.Count >= BatchSize)
            {
                await FlushBatchAsync(forceFsync: entry.Level >= LogLevel.ERROR);
                _flushDelay = Task.Delay(_flushInterval);
            }

            if (_flushDelay != null && _flushDelay.IsCompleted)
            {
                await FlushBatchAsync(forceFsync: false);
                _flushDelay = Task.Delay(_flushInterval);
            }

            if (ShouldRotate())
            {
                await RotateAsync();
            }
        }

        protected virtual void OpenNewOrExistingFile()
        {
            var timestamp = DateTime.UtcNow.ToString(_sinkTiming == SinkRoll.HOURLY ? "yyyyMMdd_HH" : "yyyMMdd");
            var path = Path.Combine(
                _filePath!,
                $"{timestamp}{(_fileName is not null ? "_id_" + _fileName : "")}" +
                $"{(FileExtension.StartsWith('.') ? FileExtension : "." + FileExtension)}");

            var dir = Path.GetDirectoryName(path)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            CloseCurrentFileSilently();

            try
            {
                _stream = new FileStream(
                        path,
                        FileMode.Append,
                        FileAccess.Write,
                        FileShare.Read,
                        bufferSize: 64 * 1024,
                        options: FileOptions.Asynchronous | FileOptions.WriteThrough);
            }
            catch (Exception)
            {
                throw new InvalidProgramException("Loggers can not write the same file!");
            }

            _writer = new StreamWriter(_stream) { AutoFlush = false };

            _nextRotationTime = DateTime.UtcNow + _rotationInterval;
            _isClosed = false;
        }

        protected virtual bool ShouldRotate()
        {
            return DateTime.UtcNow >= _nextRotationTime;
        }

        protected virtual async Task RotateAsync()
        {
            await FlushBatchAsync(forceFsync: true);
            await CloseCurrentFileAsync();
            OpenNewOrExistingFile();
        }

        protected virtual async Task CloseCurrentFileAsync()
        {
            await _ioLock.WaitAsync();

            try
            {
                if (_isClosed) return;
                _isClosed = true;

                await _writer.FlushAsync();
                _stream.Flush(true);

                await _writer.DisposeAsync();
                _stream.Dispose();
            }
            finally
            {
                _ioLock.Release();
            }
        }

        private void CloseCurrentFileSilently()
        {
            _ioLock.Wait();

            if (_isClosed) return;
            _isClosed = true;

            try
            {
                _writer?.Dispose();
                _stream?.Dispose();
            }
            catch { }
            finally
            {
                _ioLock.Release();
            }
        }

        protected sealed override async Task FlushBatchAsync()
        {
            await base.FlushBatchAsync();
        }

        protected virtual async Task FlushBatchAsync(bool forceFsync)
        {
            await _ioLock.WaitAsync();
            try
            {
                await FlushBatchAsync();

                await _writer.FlushAsync();

                if (forceFsync)
                {
                    _stream.Flush(true);
                }
            }
            finally
            {
                _ioLock.Release();
            }
        }

        // ---------- Config API ----------

        public sealed override void SetFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid file path!", nameof(filePath));

            _filePath = filePath;
        }

        public sealed override void SetFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Invalid file name!", nameof(fileName));

            _fileName = fileName;
        }

        public sealed override void SetSinkTiming(SinkRoll sinkRoll)
        {
            _sinkTiming = sinkRoll;
            _rotationInterval = TimeSpan.FromHours((int)sinkRoll);
        }

        public sealed override void SetSubDirectory(string dirName)
        {
            if (string.IsNullOrWhiteSpace(dirName))
                throw new ArgumentException("Invalid file path!", nameof(dirName));

            SubDirectory = dirName;
        }

        // ---------- Shutdown ----------

        public override async ValueTask DisposeAsync()
        {
            if (_batch.Count > 0)
                await FlushBatchAsync(forceFsync: true);

            await CloseCurrentFileAsync();

            await base.DisposeAsync();
        }


        // ---------- Getters ----------

        public sealed override string? GetFilePath() => _filePath;
        public sealed override string? GetFileName() => _fileName;
        public sealed override string? GetSubDirectory() => SubDirectory;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sentinel.Models.Options.Interfaces;
using Sentinel.Services.LogWriters;
using Sentinel.Services.LogWriters.Interfaces;

namespace Sentinel.Models.Options
{
    internal sealed class FileLogWriterOptions : IFileLogWriterOptions
    {
        private FileLogWriterBase _logWriter;
        public FileLogWriterOptions(FileLogWriterBase logWriter)
        {
            _logWriter = logWriter;
        }

        public ILogWriter GetWriterInstance()
        {
            return _logWriter;
        }

        public IFileLogWriterOptions WithLogContainerSize(int size)
        {
            _logWriter.SetLogContainerSize(size);

            return this;
        }

        public IFileLogWriterOptions WithLoggedClassFilter(string filter)
        {
            _logWriter.SetFilter(filter);

            return this;
        }

        public IFileLogWriterOptions WithMinimumLogLevel(LogLevel logLevel)
        {
            _logWriter.SetMinimiumLogLevel(logLevel);

            return this;
        }

        public IFileLogWriterOptions WithLogFileName(string fileName)
        {
            _logWriter.SetFileName(fileName);

            return this;
        }

        public IFileLogWriterOptions WithLogFilePath(string path)
        {
            _logWriter.SetFilePath(path);

            return this;
        }

        public IFileLogWriterOptions WithSinkRollTiming(SinkRoll sinkRoll)
        {
            _logWriter.SetSinkTiming(sinkRoll);

            return this;
        }

        public IFileLogWriterOptions WithLogFileSubDirectory(string subDirectory)
        {
            _logWriter.SetSubDirectory(subDirectory);

            return this;
        }
    }
}

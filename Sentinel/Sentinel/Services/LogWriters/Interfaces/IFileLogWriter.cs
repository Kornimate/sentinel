using Sentinel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services.LogWriters.Interfaces
{
    public interface IFileLogWriter
    {
        void SetFilePath(string filePath);
        void SetFileName(string fileName);
        void SetSubDirectory(string dirName);
        void SetSinkTiming(SinkRoll sinkRoll);
        string? GetFilePath();
        string? GetFileName();
        string? GetSubDirectory();
    }
}

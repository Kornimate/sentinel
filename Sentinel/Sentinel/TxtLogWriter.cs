using Sentinel.Services.LogWriters;
using Sentinel.Services.LogWriters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel
{
    public class TxtLogWriter : FileLogWriterBase
    {
        public TxtLogWriter()
        {
            FileExtension = ".txt";
            SubDirectory = "Txt";
        }
    }
}

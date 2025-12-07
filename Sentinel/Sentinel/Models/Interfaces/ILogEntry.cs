using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models.Interfaces
{
    internal interface ILogEntry
    {
        string Text { get; }
        DateTime TimeStamp { get; }
        LogLevel LogLevel { get; }
        string Serialize();
    }
}

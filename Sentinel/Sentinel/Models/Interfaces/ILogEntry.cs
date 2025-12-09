using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sentinel.Models.Interfaces
{
    public interface ILogEntry
    {
        string Text { get; }
        string FilterData { get; }

        DateTime TimeStamp { get; }
        Exception? Exception { get; }
        LogLevel Level { get; }
        string Serialize();
    }
}

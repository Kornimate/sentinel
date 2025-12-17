using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models.Options.Interfaces
{
    public interface ILogWriterOptions : ILogWriterOptionsBase
    {
        ILogWriterOptions WithLogContainerSize(int size);
        ILogWriterOptions WithLoggedClassFilter(string filter);
        ILogWriterOptions WithMinimumLogLevel(LogLevel logLevel);
    }
}

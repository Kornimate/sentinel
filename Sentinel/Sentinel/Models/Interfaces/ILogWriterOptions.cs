using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Models.Interfaces
{
    public interface ILogWriterOptions
    {
        ILogWriter GetWriterInstance();
    }
}

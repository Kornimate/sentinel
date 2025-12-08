using Sentinel.Models.Interfaces;
using Sentinel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services
{
    internal sealed class JsonLogWriter : LogWriterBase
    {
        public JsonLogWriter() : base("Json") { }
    }
}

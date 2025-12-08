using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentinel.Services
{
    internal sealed class XmlLogWriter : LogWriterBase
    {
        public XmlLogWriter() : base("Xml") { }
    }
}

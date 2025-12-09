using Sentinel.Models.LogTypes;
using Sentinel.Models.LogTypes.Interfaces;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Sentinel.Services.LogWriters
{
    internal sealed class XmlLogWriter : FileLogWriterBase
    {
        private readonly XmlSerializer _serializer;
        private readonly XmlWriterSettings _settings;
        private readonly XmlSerializerNamespaces _namespaces;

        [ThreadStatic]
        private static StringBuilder? _cachedBuilder;

        [ThreadStatic]
        private static StringWriter? _cachedStringWriter;

        public XmlLogWriter() : base()
        {
            FileExtension = ".xml";
            SubDirectory = "Xml";

            _serializer = new XmlSerializer(typeof(LogEntry));

            _settings = new XmlWriterSettings
            {
                Indent = false,
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Document,
                Async = false,
            };

            _namespaces = new XmlSerializerNamespaces();
            _namespaces.Add("", "");
        }

        protected override Task WriteLogAsync(ILogEntry entry)
        {
            string xmlString = SerializeToString((LogEntry)entry);
            return _writer.WriteLineAsync(xmlString);
        }

        private string SerializeToString(LogEntry entry)
        {
            var sb = _cachedBuilder ??= new StringBuilder(512);
            sb.Clear();

            var sw = _cachedStringWriter;
            if (sw == null)
            {
                sw = new StringWriter(sb);
                _cachedStringWriter = sw;
            }
            else
            {
                sw.GetStringBuilder().Clear();
            }

            _serializer.Serialize(sw, entry, _namespaces);

            return TrimXmlHeader(sb.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // optimize by compiler
        private static string TrimXmlHeader(string xml)
        {
            int end = xml.IndexOf("?>", StringComparison.Ordinal);
            if (end == -1)
                return xml;
            return xml[(end + 2)..].TrimStart();
        }
    }
}

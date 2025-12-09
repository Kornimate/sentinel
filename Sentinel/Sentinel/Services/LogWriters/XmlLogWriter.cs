using Sentinel.Models.LogTypes;
using Sentinel.Models.LogTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
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
        private XmlWriter? _xmlWriter;
        public XmlLogWriter() : base()
        {
            FileExtension = ".xml";
            SubDirectory = "Xml";

            var overrides = new XmlAttributeOverrides();

            // ---------- LogEntry attributes mappings ----------

            // TimeStamp -> ts="ISO-8601"
            var tsAttr = new XmlAttributes();
            tsAttr.XmlAttribute = new XmlAttributeAttribute("ts");
            overrides.Add(typeof(LogEntry), nameof(LogEntry.TimeStamp), tsAttr);

            // Level -> level="Error"
            var levelAttr = new XmlAttributes();
            levelAttr.XmlAttribute = new XmlAttributeAttribute("level");
            overrides.Add(typeof(LogEntry), nameof(LogEntry.Level), levelAttr);

            // Text -> inner text of <log>
            var textAttr = new XmlAttributes();
            textAttr.XmlText = new XmlTextAttribute();
            overrides.Add(typeof(LogEntry), nameof(LogEntry.Text), textAttr);

            // Exception -> <exception>...</exception>
            var exAttr = new XmlAttributes();
            exAttr.XmlElements.Add(new XmlElementAttribute("exception"));
            overrides.Add(typeof(LogEntry), nameof(LogEntry.Exception), exAttr);

            // FilterData -> omit
            var ignoreFilter = new XmlAttributes { XmlIgnore = true };
            overrides.Add(typeof(LogEntry), nameof(LogEntry.FilterData), ignoreFilter);

            // Serialize() -> omit
            var ignoreSerialize = new XmlAttributes { XmlIgnore = true };
            overrides.Add(typeof(LogEntry), nameof(LogEntry.Serialize), ignoreSerialize);

            // ---------- System.Exception mappings ----------

            var exMessage = new XmlAttributes();
            exMessage.XmlElements.Add(new XmlElementAttribute("message"));
            overrides.Add(typeof(Exception), nameof(Exception.Message), exMessage);

            var exStack = new XmlAttributes();
            exStack.XmlElements.Add(new XmlElementAttribute("stackTrace"));
            overrides.Add(typeof(Exception), nameof(Exception.StackTrace), exStack);

            var exSource = new XmlAttributes();
            exSource.XmlElements.Add(new XmlElementAttribute("source"));
            overrides.Add(typeof(Exception), nameof(Exception.Source), exSource);

            var exHresult = new XmlAttributes();
            exHresult.XmlElements.Add(new XmlElementAttribute("hresult"));
            overrides.Add(typeof(Exception), nameof(Exception.HResult), exHresult);

            var exInner = new XmlAttributes();
            exInner.XmlElements.Add(new XmlElementAttribute("innerException"));
            overrides.Add(typeof(Exception), nameof(Exception.InnerException), exInner);

            // ---------- Root ----------
            var root = new XmlRootAttribute("log");

            _serializer = new XmlSerializer(typeof(LogEntry), overrides, null, root, null);

            _settings = new XmlWriterSettings
            {
                Indent = false,
                OmitXmlDeclaration = true,
                CloseOutput = false
            };

            _namespaces = new XmlSerializerNamespaces();
            _namespaces.Add("", "");
        }

        protected override Task WriteLogAsync(ILogEntry entry)
        {
            _xmlWriter ??= XmlWriter.Create(_stream, _settings);

            _serializer.Serialize(_xmlWriter, new
            {
                Level = entry.Level.ToString(),
                TimeStamp = entry.TimeStamp.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'"),
                Message = entry.Text,
                Exception = entry.Exception is null ? "" : entry.Exception.ToString(),
            }, _namespaces);

            _serializer.Serialize(_writer, entry, _namespaces);
            _xmlWriter.WriteWhitespace("\n");
            _writer.Flush();

            return Task.CompletedTask;
        }

        public override ValueTask DisposeAsync()
        {
            _xmlWriter?.Dispose();

            return base.DisposeAsync();
        }
    }
}

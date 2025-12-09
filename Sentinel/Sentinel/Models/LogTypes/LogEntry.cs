using Sentinel.Models.LogTypes.Interfaces;
using System;
using System.Xml.Serialization;

namespace Sentinel.Models.LogTypes
{
    [XmlRoot("log")]
    public sealed record LogEntry : ILogEntry
    {
        public static ILogEntry CreateLogEntry(
            string message,
            string caller = "",
            LogLevel logLevel = LogLevel.VERBS,
            Exception? exception = null)
        {
            return new LogEntry
            {
                Text = message,
                Level = logLevel,
                Exception = exception,
                FilterData = caller,
                TimeStamp = DateTime.UtcNow
            };
        }

        [XmlText]
        public string Text { get; set; } = string.Empty;

        [XmlIgnore]
        public string FilterData { get; set; } = string.Empty;

        [XmlAttribute("ts")]
        public string TimeStampString
        {
            get => TimeStamp.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
            set { } // required for XmlSerializer
        }

        [XmlIgnore]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute("level")]
        public string LevelString
        {
            get => Level.ToString();
            set { }
        }

        [XmlIgnore]
        public LogLevel Level { get; set; }

        [XmlIgnore]
        public Exception? Exception { get; set; }

        [XmlElement("exception")]
        public string? ExceptionText
        {
            get => Exception?.ToString() ?? "-";
            set { }
        }
        public string Serialize()
        {
            return $"{TimeStamp:yyyy-MM-dd'T'HH:mm:ss.fff'Z'} [{Level}]: {Text}" + (Exception is null ? "" : ", " + Exception);
        }
    }
}

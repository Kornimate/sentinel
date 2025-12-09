using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sentinel.Models;
using Sentinel.Models.LogTypes.Interfaces;
using Sentinel.Services.LogWriters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sentinel.Tests
{
    [TestClass]
    public class ConsoleLogWriterBaseTest
    {
        private sealed class TestConsoleLogWriter : ConsoleLogWriterBase
        {
            public readonly List<ILogEntry> Written = new();

            protected override Task WriteLogAsync(ILogEntry entry)
            {
                Written.Add(entry);
                return base.WriteLogAsync(entry);
            }
        }

        private sealed class TestLogEntry : ILogEntry
        {
            public LogLevel Level { get; set; }
            public string FilterData { get; set; } = string.Empty;

            public string Text => throw new NotImplementedException();

            public DateTime TimeStamp => throw new NotImplementedException();

            public Exception? Exception => throw new NotImplementedException();

            public string Serialize() => $"[{Level}] test";
        }

        private StringWriter _consoleOut = default!;

        [TestInitialize]
        public void Setup()
        {
            _consoleOut = new StringWriter();
            Console.SetOut(_consoleOut);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _consoleOut.Dispose();
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        }

        [TestMethod]
        public void WriteToConsole_Returns_True()
        {
            var writer = new TestConsoleLogWriter();

            var result = writer.WriteToConsole();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Build_Starts_Background_Task()
        {
            var writer = new TestConsoleLogWriter();

            writer.Build();
            var task = writer.GetBackgroundConsumerTask();

            Assert.IsNotNull(task);
            Assert.IsFalse(task.IsCompleted);
        }

        [TestMethod]
        public async Task AddLogMessage_Writes_To_Console()
        {
            var writer = new TestConsoleLogWriter();
            writer.Build();

            var entry = new TestLogEntry { Level = LogLevel.INFOR };
            writer.AddLogMessage(this, entry);

            await Task.Delay(200);

            var output = _consoleOut.ToString();

            Assert.IsTrue(output.Contains("INFO"));
            Assert.AreEqual(1, writer.Written.Count);
        }

        [TestMethod]
        public async Task Minimum_LogLevel_Is_Respected()
        {
            var writer = new TestConsoleLogWriter();
            writer.SetMinimiumLogLevel(LogLevel.ERROR);
            writer.Build();

            var entry = new TestLogEntry { Level = LogLevel.INFOR };
            writer.AddLogMessage(this, entry);

            await Task.Delay(200);

            Assert.AreEqual(0, writer.Written.Count);
            Assert.AreEqual(string.Empty, _consoleOut.ToString());
        }

        [TestMethod]
        public async Task Filter_Is_Respected()
        {
            var writer = new TestConsoleLogWriter();
            writer.SetFilter("Console");
            writer.Build();

            var entry = new TestLogEntry { Level = LogLevel.INFOR };
            writer.AddLogMessage(this, entry);

            await Task.Delay(200);

            Assert.AreEqual(0, writer.Written.Count);
            Assert.AreEqual(string.Empty, _consoleOut.ToString());
        }

        [TestMethod]
        public async Task DisposeAsync_Stops_Consumer()
        {
            var writer = new TestConsoleLogWriter();
            writer.Build();

            await writer.DisposeAsync();

            var task = writer.GetBackgroundConsumerTask();

            Assert.IsTrue(task!.IsCompleted);
        }

        [TestMethod]
        public void ShutDown_Is_Idempotent()
        {
            var writer = new TestConsoleLogWriter();
            writer.Build();

            writer.ShutDown();
            writer.ShutDown();

            var task = writer.GetBackgroundConsumerTask();

            Assert.IsTrue(task!.IsCompleted);
        }
    }
}

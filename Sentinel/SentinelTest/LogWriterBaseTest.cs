using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sentinel.Models;
using Sentinel.Models.LogTypes.Interfaces;
using Sentinel.Services.LogWriters;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sentinel.Tests
{
    [TestClass]
    public class LogWriterBaseTest
    {
        private sealed class TestLogWriter : LogWriterBase
        {
            public readonly List<ILogEntry> Consumed = new();

            protected override async Task ConsumeAsync(CancellationToken token)
            {
                await foreach (var entry in _channel!.Reader.ReadAllAsync(token))
                {
                    Consumed.Add(entry);
                    await WriteLogAsync(entry);
                }
            }

            protected override Task WriteLogAsync(ILogEntry entry)
            {
                return Task.CompletedTask;
            }

            public void SetMinimum(LogLevel level)
            {
                _minimumLevel = level;
            }

            public void SetInternalFilter(string filter)
            {
                _filter = filter;
            }
        }

        private sealed class TestLogEntry : ILogEntry
        {
            public LogLevel Level { get; set; }
            public string FilterData { get; set; } = string.Empty;

            public string Text => throw new NotImplementedException();

            public DateTime TimeStamp => throw new NotImplementedException();

            public Exception? Exception => throw new NotImplementedException();

            public string Serialize() => $"{Level}";
        }

        [TestMethod]
        public void Build_Initializes_Channel_And_Background_Task()
        {
            var writer = new TestLogWriter();

            writer.Build();
            var task = writer.GetBackgroundConsumerTask();

            Assert.IsNotNull(task);
            Assert.IsFalse(task.IsCompleted);
        }

        [TestMethod]
        public void Build_Is_Idempotent()
        {
            var writer = new TestLogWriter();

            var first = writer.Build();
            var second = writer.Build();

            Assert.AreSame(first, second);
        }

        [TestMethod]
        public async Task AddLogMessage_Writes_To_Channel_And_Consumes()
        {
            var writer = new TestLogWriter();
            writer.Build();

            var entry = new TestLogEntry { Level = LogLevel.INFOR };
            writer.AddLogMessage(this, entry);

            await Task.Delay(200);

            Assert.AreEqual(1, writer.Consumed.Count);
        }

        [TestMethod]
        public async Task AddLogMessage_Respects_Minimum_LogLevel()
        {
            var writer = new TestLogWriter();
            writer.SetMinimum(LogLevel.ERROR);
            writer.Build();

            var entry = new TestLogEntry { Level = LogLevel.INFOR };
            writer.AddLogMessage(this, entry);

            await Task.Delay(200);

            Assert.AreEqual(0, writer.Consumed.Count);
        }

        [TestMethod]
        public async Task AddLogMessage_Respects_Filter()
        {
            var writer = new TestLogWriter();
            writer.SetInternalFilter("Allow");
            writer.Build();

            var entry = new TestLogEntry { Level = LogLevel.INFOR, FilterData = "Blocked" };
            writer.AddLogMessage(this, entry);

            await Task.Delay(200);

            Assert.AreEqual(0, writer.Consumed.Count);
        }

        [TestMethod]
        public async Task DisposeAsync_Stops_Background_Task()
        {
            var writer = new TestLogWriter();
            writer.Build();

            await writer.DisposeAsync();

            var task = writer.GetBackgroundConsumerTask();

            Assert.IsTrue(task!.IsCompleted);
        }

        [TestMethod]
        public void ShutDown_Is_Idempotent()
        {
            var writer = new TestLogWriter();
            writer.Build();

            writer.ShutDown();
            writer.ShutDown();

            var task = writer.GetBackgroundConsumerTask();

            Assert.IsTrue(task!.IsCompleted);
        }

        [TestMethod]
        public void Default_WriteToConsole_Returns_False()
        {
            var writer = new TestLogWriter();

            var result = writer.WriteToConsole();

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void SetFilePath_Throws_By_Default()
        {
            var writer = new TestLogWriter();
            writer.SetFilePath("test");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void SetFileName_Throws_By_Default()
        {
            var writer = new TestLogWriter();
            writer.SetFileName("test");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void SetSinkTiming_Throws_By_Default()
        {
            var writer = new TestLogWriter();
            writer.SetSinkTiming(SinkRoll.DAILY);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void SetSubDirectory_Throws_By_Default()
        {
            var writer = new TestLogWriter();
            writer.SetSubDirectory("test");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetFilePath_Throws_By_Default()
        {
            var writer = new TestLogWriter();
            _ = writer.GetFilePath();
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetFileName_Throws_By_Default()
        {
            var writer = new TestLogWriter();
            _ = writer.GetFileName();
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetSubDirectory_Throws_By_Default()
        {
            var writer = new TestLogWriter();
            _ = writer.GetSubDirectory();
        }
    }
}

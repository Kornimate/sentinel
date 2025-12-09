using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sentinel.Models;
using Sentinel.Models.LogTypes.Interfaces;
using Sentinel.Services.LogWriters;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sentinel.Tests
{
    [TestClass]
    public class FileLogWriterBaseTest
    {
        private sealed class TestFileLogWriter : FileLogWriterBase
        {
            public Task InvokeWriteEntry(ILogEntry entry) => WriteEntryToFileOrBatch(entry);
            public bool InvokeShouldRotate() => ShouldRotate();
        }

        private sealed class TestLogEntry : ILogEntry
        {
            public LogLevel Level { get; set; }

            public string Text => throw new NotImplementedException();

            public string FilterData => throw new NotImplementedException();

            public DateTime TimeStamp => throw new NotImplementedException();

            public Exception? Exception => throw new NotImplementedException();

            public string Serialize() => $"[{Level}] test";
        }

        private string _tempDir = default!;

        [TestInitialize]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetFilePath_Invalid_Throws()
        {
            var writer = new TestFileLogWriter();
            writer.SetFilePath(" ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetFileName_Invalid_Throws()
        {
            var writer = new TestFileLogWriter();
            writer.SetFileName("");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetSubDirectory_Invalid_Throws()
        {
            var writer = new TestFileLogWriter();
            writer.SetSubDirectory(" ");
        }

        [TestMethod]
        public void Build_Creates_Log_File()
        {
            var writer = new TestFileLogWriter();
            writer.SetFilePath(_tempDir);
            writer.SetSubDirectory("Logs");
            writer.SetFileName("test");

            writer.Build();

            var logDir = Path.Combine(_tempDir, "Logs");
            Assert.IsTrue(Directory.Exists(logDir));
            Assert.IsTrue(Directory.GetFiles(logDir).Length == 1);

            writer.ShutDown();
        }

        [TestMethod]
        public async Task WriteEntry_Writes_To_File()
        {
            var writer = new TestFileLogWriter();
            writer.SetFilePath(_tempDir);
            writer.SetSubDirectory("Logs");
            writer.SetFileName("write");
            writer.Build();

            var entry = new TestLogEntry { Level = LogLevel.INFOR };
            await writer.InvokeWriteEntry(entry);
            await writer.DisposeAsync();

            var file = Directory.GetFiles(Path.Combine(_tempDir, "Logs"))[0];
            var content = await File.ReadAllTextAsync(file);

            Assert.IsTrue(content.Contains("INFO"));
        }

        [TestMethod]
        public async Task Error_Level_Forces_Immediate_Flush()
        {
            var writer = new TestFileLogWriter();
            writer.SetFilePath(_tempDir);
            writer.SetSubDirectory("Logs");
            writer.SetFileName("error");
            writer.Build();

            var entry = new TestLogEntry { Level = LogLevel.ERROR };
            await writer.InvokeWriteEntry(entry);

            writer.ShutDown();

            var file = Directory.GetFiles(Path.Combine(_tempDir, "Logs"))[0];
            var content = await File.ReadAllTextAsync(file);

            Assert.IsTrue(content.Contains("ERROR"));
        }

        [TestMethod]
        public void ShouldRotate_Returns_True_When_Time_Exceeded()
        {
            var writer = new TestFileLogWriter();
            writer.SetFilePath(_tempDir);
            writer.SetSubDirectory("Logs");
            writer.SetFileName("rotate");
            writer.Build();

            var field = typeof(FileLogWriterBase).GetField("_nextRotationTime", BindingFlags.Instance | BindingFlags.NonPublic);
            field!.SetValue(writer, DateTime.UtcNow.AddSeconds(-1));

            var result = writer.InvokeShouldRotate();

            Assert.IsTrue(result);

            writer.ShutDown();
        }

        [TestMethod]
        public async Task DisposeAsync_Flushes_And_Closes_File()
        {
            var writer = new TestFileLogWriter();
            writer.SetFilePath(_tempDir);
            writer.SetSubDirectory("Logs");
            writer.SetFileName("dispose");
            writer.Build();

            var entry = new TestLogEntry { Level = LogLevel.INFOR };
            await writer.InvokeWriteEntry(entry);
            await writer.DisposeAsync();

            var file = Directory.GetFiles(Path.Combine(_tempDir, "Logs"))[0];
            var content = await File.ReadAllTextAsync(file);

            Assert.IsTrue(content.Length > 0);
        }
    }
}

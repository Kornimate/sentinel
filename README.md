<p align="center">
<h1>Sentinel</h1>
</p>
<p align="center">
<img src="Images/sentinel_logo.png" width="300">`{=html}
</p>

---

# Sentinel Logger -- Complete Usage Guide

This document describes the architecture, configuration, and correct
usage of all logging writers in the Sentinel logging system. It covers:

- `LogWriterBase`
- `FileLogWriterBase`
- `ConsoleLogWriterBase`
- How to create custom derived writers
- Filtering, batching, rotation, and shutdown behavior

This README is intended for engineers implementing or extending the
logging system.

---

## 1. Architecture Overview

The logging system is structured using three layers of abstraction:

    ILogWriter
       ↑
    LogWriterBase
       ↑
    +----------------------+
    |                      |
    FileLogWriterBase   ConsoleLogWriterBase

### Responsibilities

---

Layer Responsibility

---

`LogWriterBase` Async channel, filtering, background task
lifecycle

`FileLogWriterBase` File handling, batching, flushing, rotation

`ConsoleLogWriterBase` Console output only

---

> You never instantiate base classes directly. You always create derived
> implementations.

---

## 2. LogWriterBase -- Core Behavior

`LogWriterBase` provides:

- Async log buffering using `Channel<T>`
- Background consumer task
- Log filtering and minimum level rules
- Graceful shutdown handling
- Backpressure handling when the channel is full

### Internal Pipeline

    AddLogMessage()
       ↓
    Channel<ILogEntry>
       ↓
    Background Task
       ↓
    ConsumeAsync()
       ↓
    WriteLogAsync()

---

## 3. Writer Lifecycle

### 3.1 Construction with Builder

```csharp
var builder = LoggerBuilder.CreateLogger(options =>
{
    options
        .AddConsoleLogger()
        .AddJsonLogger()
        .AddJsonLogger(options =>
        {
            options
            .WithLogFilePath("D:\\Logs")
            .WithLoggedClassFilter("Program")
            .WithLogFileName("Test")
            .WithMinimumLogLevel(LogLevel.WARNG)
            .WithSinkRollTiming(SinkRoll.DAILY);
        })
        .AddXmlLogger()
        .AddXmlLogger();
        //.AddCustomLogger<>();
});

var logger = builder.GetLogger<Program>();
```

#### 3.1.1 Usage of class

```csharp
logger.Log(Models.LogLevel.VERBS, "Hello 1");
logger.LogDebug("Hello 2");
logger.LogInformation("Hello 3");
logger.LogWarning("Hello 4");
logger.LogError("Hello 5");
logger.LogFatal("Hello 6");
```

### 3.2 Configuration of Writers

Before calling `Build()`:

```csharp
writer.SetMinimiumLogLevel(LogLevel.INFO);
writer.SetFilter("MyService");
```

For file writers:

```csharp
writer.SetFilePath("C:\Logs");
writer.SetSubDirectory("App");
writer.SetFileName("runtime");
writer.SetSinkTiming(SinkRoll.DAILY);
```

### 3.3 Build

```csharp
writer.Build();
```

### 3.4 Logging

```csharp
writer.AddLogMessage(this, logEntry);
```

### 3.5 Shutdown

```csharp
await writer.DisposeAsync();
```

---

## 13. What This System Guarantees

- High-throughput async logging
- No log loss under normal operation
- Safe shutdown with full drain
- Fully extensible formatting and rotation logic
- Production-grade file safety

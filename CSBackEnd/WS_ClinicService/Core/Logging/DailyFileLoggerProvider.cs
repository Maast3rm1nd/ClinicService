using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace WS_ClinicService.Core.Logging
{
    public sealed class DailyFileLoggerProvider : ILoggerProvider
    {
        private readonly object _sync = new();
        private readonly string _logDirectory;
        private readonly string _serviceName;
        private StreamWriter? _writer;
        private DateOnly _currentDate;
        private string? _currentPath;
        private bool _disposed;

        public DailyFileLoggerProvider(string? serviceName = null)
        {
            _serviceName = string.IsNullOrWhiteSpace(serviceName)
                ? Assembly.GetEntryAssembly()?.GetName().Name ?? "ClinicService"
                : serviceName;
            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            Directory.CreateDirectory(_logDirectory);
        }

        public ILogger CreateLogger(string categoryName) => new DailyFileLogger(this, categoryName);

        internal void Write(LogLevel logLevel, string categoryName, EventId eventId, string message, Exception? exception)
        {
            if (logLevel == LogLevel.None)
            {
                return;
            }

            lock (_sync)
            {
                if (_disposed)
                {
                    return;
                }

                EnsureWriter();
                var timestamp = DateTime.Now;
                _writer!.WriteLine($"{timestamp:dd.MM.yyyy HH:mm:ss} [{GetLevelTag(logLevel)}] {categoryName} ({eventId.Id}) {message}");
                if (exception is not null)
                {
                    _writer.WriteLine(exception);
                }
                _writer.Flush();
            }
        }

        private static string GetLevelTag(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Trace => "TRC",
            LogLevel.Debug => "DBG",
            LogLevel.Information => "INF",
            LogLevel.Warning => "WRN",
            LogLevel.Error => "ERR",
            LogLevel.Critical => "CRT",
            _ => logLevel.ToString().ToUpperInvariant()
        };

        private void EnsureWriter()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            if (_writer is not null && today == _currentDate)
            {
                return;
            }

            var previousPath = _writer is null ? null : _currentPath;
            _writer?.Dispose();
            if (previousPath is not null && File.Exists(previousPath))
            {
                Archive(previousPath);
            }

            _currentDate = today;
            var (stream, path, isNew) = OpenWriterStream(today);
            _currentPath = path;

            _writer = new StreamWriter(stream, Encoding.UTF8)
            {
                AutoFlush = true
            };
            if (isNew)
            {
                WriteHeader(_writer);
            }
        }

        private (FileStream Stream, string Path, bool IsNew) OpenWriterStream(DateOnly date)
        {
            const int maxAttemptsPerFile = 5;
            const int maxFileIndex = 100;

            for (var index = 0; index <= maxFileIndex; index++)
            {
                var path = GetLogPath(date, index);
                var isNew = !File.Exists(path) || new FileInfo(path).Length == 0;
                var canFallBack = index < maxFileIndex;

                for (var attempt = 1; attempt <= maxAttemptsPerFile; attempt++)
                {
                    try
                    {
                        var stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        return (stream, path, isNew);
                    }
                    catch (IOException) when (attempt < maxAttemptsPerFile)
                    {
                        Thread.Sleep(100 * attempt);
                    }
                    catch (IOException) when (canFallBack)
                    {
                        // Could not access this file even after retries; fall back to the next incremented file name.
                        break;
                    }
                }
            }

            throw new IOException($"Unable to access any log file variant for '{GetLogPath(date, 0)}' after {maxFileIndex} attempts.");
        }

        private string GetLogPath(DateOnly date, int index) => index == 0
            ? Path.Combine(_logDirectory, $"{_serviceName}-{date:yyyy-MM-dd}.log")
            : Path.Combine(_logDirectory, $"{_serviceName}-{date:yyyy-MM-dd} ({index}).log");

        private void Archive(string path)
        {
            var archivePath = Path.ChangeExtension(path, ".zip");
            if (File.Exists(archivePath))
            {
                File.Delete(archivePath);
            }

            using var archive = ZipFile.Open(archivePath, ZipArchiveMode.Create);
            archive.CreateEntryFromFile(path, Path.GetFileName(path), CompressionLevel.Optimal);
            File.Delete(path);
        }

        private void WriteHeader(StreamWriter writer)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            writer.WriteLine("================================================================");
            writer.WriteLine($"Service: {_serviceName}");
            writer.WriteLine($"Version: {entryAssembly?.GetName().Version?.ToString() ?? "unknown"}");
            writer.WriteLine($"Started: {DateTimeOffset.Now:O}");
            writer.WriteLine("Dependencies:");

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .Select(assembly => new
                {
                    Name = assembly.GetName().Name ?? "unknown",
                    Version = assembly.GetName().Version?.ToString() ?? "unknown"
                })
                .OrderBy(item => item.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var assembly in assemblies)
            {
                writer.WriteLine($"  - {assembly.Name}: {assembly.Version}");
            }

            writer.WriteLine("================================================================");
        }

        public void Dispose()
        {
            lock (_sync)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _writer?.Dispose();
                _writer = null;
            }
        }

        private sealed class DailyFileLogger(DailyFileLoggerProvider provider, string categoryName) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                provider.Write(logLevel, categoryName, eventId, formatter(state, exception), exception);
            }
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}

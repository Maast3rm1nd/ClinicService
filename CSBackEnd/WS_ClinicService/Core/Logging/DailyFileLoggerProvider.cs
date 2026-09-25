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
                var timestamp = DateTimeOffset.Now;
                _writer!.WriteLine($"{timestamp:O} [{logLevel}] {categoryName} ({eventId.Id}) {message}");
                if (exception is not null)
                {
                    _writer.WriteLine(exception);
                }
                _writer.Flush();
            }
        }

        private void EnsureWriter()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            if (_writer is not null && today == _currentDate)
            {
                return;
            }

            var previousPath = _writer is null ? null : GetLogPath(_currentDate);
            _writer?.Dispose();
            if (previousPath is not null && File.Exists(previousPath))
            {
                Archive(previousPath);
            }

            _currentDate = today;
            var path = GetLogPath(today);
            var isNew = !File.Exists(path) || new FileInfo(path).Length == 0;
            _writer = new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read), Encoding.UTF8)
            {
                AutoFlush = true
            };
            if (isNew)
            {
                WriteHeader(_writer);
            }
        }

        private string GetLogPath(DateOnly date) => Path.Combine(_logDirectory, $"{_serviceName}-{date:yyyy-MM-dd}.log");

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

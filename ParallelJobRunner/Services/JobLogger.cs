using System.Collections.Concurrent;
using ParallelJobRunner.Interfaces;
using ParallelJobRunner.Logs;

namespace ParallelJobRunner.Services
{
    /// <summary>
    /// Provides logging functionality for background jobs, storing logs in memory and a file.
    /// </summary>
    public class JobLogger : IJobLogger, IDisposable
    {
        private readonly ConcurrentDictionary<string, List<JobLogEntry>> _logs = new();
        private readonly string _logFilePath;
        private readonly StreamWriter _streamWriter;
        private readonly object _fileLock = new();

        /// <summary>
        /// Occurs when a new log entry is added.
        /// </summary>
        public event Action<string, JobLogEntry>? LogAdded;

        /// <summary>
        /// Gets the file path where logs are stored.
        /// </summary>
        public string LogFilePath => _logFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobLogger"/> class.
        /// </summary>
        public JobLogger()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _logFilePath = Path.Combine(Path.GetTempPath(), $"ParallelJobs_{timestamp}.txt");

            _streamWriter = new StreamWriter(new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                AutoFlush = true
            };
        }

        public void Log(string jobId, string message)
        {
            var entry = new JobLogEntry { Message = message };
            var list = _logs.GetOrAdd(jobId, _ => new List<JobLogEntry>());

            lock (list)
            {
                list.Add(entry);
            }

            LogAdded?.Invoke(jobId, entry);

            lock (_fileLock)
                _streamWriter.WriteLine($"{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{jobId}] {entry.Message}");
        }

        public IReadOnlyList<JobLogEntry> GetLogs(string jobId)
        {
            return _logs.TryGetValue(jobId, out var logs) ? logs.AsReadOnly() : Array.Empty<JobLogEntry>();
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
        }
    }
}

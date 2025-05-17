using ParallelJobRunner.Logs;

namespace ParallelJobRunner.Interfaces
{
    /// <summary>
    /// Defines a logger for recording job-related events and messages.
    /// </summary>
    public interface IJobLogger
    {
        /// <summary>
        /// Logs a message for a specific job.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <param name="message">The message to log.</param>
        void Log(string jobId, string message);

        /// <summary>
        /// Retrieves all log entries for a specific job.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job.</param>
        /// <returns>A read-only list of log entries.</returns>
        IReadOnlyList<JobLogEntry> GetLogs(string jobId);

        /// <summary>
        /// Occurs when a new log entry is added.
        /// </summary>
        event Action<string, JobLogEntry> LogAdded;

        /// <summary>
        /// Gets the file path where logs are stored.
        /// </summary>
        string LogFilePath { get; }
    }
}
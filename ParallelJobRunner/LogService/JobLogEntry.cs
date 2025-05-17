namespace ParallelJobRunner.Logs
{
    /// <summary>
    /// Represents a single log entry for a job, containing a timestamp and message.
    /// </summary>
    public class JobLogEntry
    {
        /// <summary>
        /// Gets or sets the timestamp of the log entry.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the message of the log entry.
        /// </summary>
        public string Message { get; set; }
    }
}

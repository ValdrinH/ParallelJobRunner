using ParallelJobRunner.Interfaces;
using ParallelJobRunner.Logs;

namespace ParallelJobRunner.Models
{
    /// <summary>
    /// Represents a response containing a job, its logs, and its result.
    /// </summary>
    public class JobResponse
    {
        /// <summary>
        /// Gets or sets the job wrapper.
        /// </summary>
        public IBackgroundJobWrapper Job { get; set; }

        /// <summary>
        /// Gets or sets the list of log entries for the job.
        /// </summary>
        public IReadOnlyList<JobLogEntry> Logs { get; set; }

        /// <summary>
        /// Gets or sets the result of the job, if completed.
        /// </summary>
        public object? Result { get; set; }
    }
}
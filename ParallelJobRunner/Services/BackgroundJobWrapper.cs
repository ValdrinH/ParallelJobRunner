using ParallelJobRunner.Interfaces;

namespace ParallelJobRunner.Services
{/// <summary>
 /// Wraps a <see cref="BackgroundJob{T}"/> to provide a non-generic interface for job management.
 /// </summary>
 /// <typeparam name="T">The type of the job's return value.</typeparam>
    public class BackgroundJobWrapper<T> : IBackgroundJobWrapper
    {
        /// <summary>
        /// Gets the underlying background job.
        /// </summary>
        public BackgroundJob<T> Job { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundJobWrapper{T}"/> class.
        /// </summary>
        /// <param name="job">The background job to wrap.</param>
        public BackgroundJobWrapper(BackgroundJob<T> job) => Job = job;

        /// <summary>
        /// Gets the unique identifier of the job.
        /// </summary>
        public Guid JobId => Job.JobId;

        /// <summary>
        /// Gets the name of the job.
        /// </summary>
        public string Name => Job.Name;

        /// <summary>
        /// Gets the current status of the job.
        /// </summary>
        public string Status => Job.Status;

        /// <summary>
        /// Gets the result of the job, if completed.
        /// </summary>
        public object? Result => Job.Result;

        /// <summary>
        /// Cancels the job if it is running.
        /// </summary>
        public void Cancel() => Job.Cancel();

        /// <summary>
        /// Pauses the job if it is running.
        /// </summary>
        public void Pause() => Job.Pause();

        /// <summary>
        /// Resumes the job if it is paused.
        /// </summary>
        public void Resume() => Job.Resume();

        /// <summary>
        /// Returns a string representation of the job.
        /// </summary>
        /// <returns>A string containing the job's name and status.</returns>
        public override string ToString() => $"{Name} - {Status}";
    }
}

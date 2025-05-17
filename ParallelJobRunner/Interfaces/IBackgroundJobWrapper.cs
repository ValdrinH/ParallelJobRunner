namespace ParallelJobRunner.Interfaces
{
    /// <summary>
    /// Defines a wrapper for a background job, providing access to its metadata and control methods.
    /// </summary>
    public interface IBackgroundJobWrapper
    {
        /// <summary>
        /// Gets the unique identifier of the job.
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// Gets the name of the job.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the current status of the job.
        /// </summary>
        string Status { get; }

        /// <summary>
        /// Gets the result of the job, if completed.
        /// </summary>
        object? Result { get; }

        /// <summary>
        /// Cancels the job if it is running.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Pauses the job if it is running.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the job if it is paused.
        /// </summary>
        void Resume();
    }
}
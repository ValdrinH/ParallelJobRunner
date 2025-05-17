using System.Collections.ObjectModel;
using ParallelJobRunner.Models;
using ParallelJobRunner.Services;

namespace ParallelJobRunner.Interfaces
{
    /// <summary>
    /// Defines a manager for handling background jobs, including adding, cancelling, and retrieving job information.
    /// </summary>
    public interface IJobManager
    {
        /// <summary>
        /// Gets the collection of background jobs.
        /// </summary>
        ObservableCollection<IBackgroundJobWrapper> Jobs { get; }

        /// <summary>
        /// Adds a new background job to the manager.
        /// </summary>
        /// <typeparam name="T">The type of the job's return value.</typeparam>
        /// <param name="job">The background job to add.</param>
        void AddJob<T>(BackgroundJob<T> job);

        /// <summary>
        /// Cancels all running jobs.
        /// </summary>
        void CancelAll();

        /// <summary>
        /// Pauses a specific job by its ID.
        /// </summary>
        /// <param name="jobId">The ID of the job to pause.</param>
        void PauseJob(Guid jobId);

        /// <summary>
        /// Resumes a specific job by its ID.
        /// </summary>
        /// <param name="jobId">The ID of the job to resume.</param>
        void ResumeJob(Guid jobId);

        /// <summary>
        /// Occurs when a new job is added.
        /// </summary>
        event Action<IBackgroundJobWrapper>? JobAdded;

        /// <summary>
        /// Occurs when a job completes.
        /// </summary>
        event Action<IBackgroundJobWrapper>? JobCompleted;

        /// <summary>
        /// Retrieves all jobs along with their associated logs and results.
        /// </summary>
        /// <returns>An enumerable collection of job responses.</returns>
        IEnumerable<JobResponse> GetAllJobsWithLogs();

        /// <summary>
        /// Determines whether there are any running jobs.
        /// </summary>
        /// <returns>True if there are running jobs; otherwise, false.</returns>
        bool HasRunningJobs();

        /// <summary>
        /// Cancels all running jobs and waits for them to complete or cancel.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CancelOrWaitForRunningJobsAsync();
    }
}
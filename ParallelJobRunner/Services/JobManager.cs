using System.Collections.ObjectModel;
using ParallelJobRunner;
using ParallelJobRunner.Interfaces;
using ParallelJobRunner.Models;
using ParallelJobRunner.Services;

public class JobManager : IJobManager
{
    private readonly ObservableCollection<IBackgroundJobWrapper> _jobs = new();
    private readonly object _lock = new(); // For thread-safe access

    /// <summary>
    /// Gets the collection of background jobs.
    /// </summary>
    public ObservableCollection<IBackgroundJobWrapper> Jobs => _jobs;

    /// <summary>
    /// Occurs when a new job is added.
    /// </summary>
    public event Action<IBackgroundJobWrapper>? JobAdded;

    /// <summary>
    /// Occurs when a job completes.
    /// </summary>
    public event Action<IBackgroundJobWrapper>? JobCompleted;

    private readonly IJobLogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobManager"/> class.
    /// </summary>
    /// <param name="logger">The logger for job events. If null, uses <see cref="JobLoggerProvider.Instance"/>.</param>
    public JobManager(IJobLogger? logger = null)
    {
        _logger = logger ?? JobLoggerProvider.Instance;
    }

    /// <summary>
    /// Adds a new background job to the manager.
    /// </summary>
    /// <typeparam name="T">The type of the job's return value.</typeparam>
    /// <param name="job">The background job to add.</param>
    public void AddJob<T>(BackgroundJob<T> job)
    {
        var jobWrapper = new BackgroundJobWrapper<T>(job);

        lock (_lock)
        {
            _jobs.Add(jobWrapper);
        }

        JobAdded?.Invoke(jobWrapper);

        _ = Task.Run(async () =>
        {
            await job.RunAsync();
            JobCompleted?.Invoke(jobWrapper);
        });
    }

    /// <summary>
    /// Cancels all running jobs.
    /// </summary>
    public void CancelAll()
    {
        lock (_lock)
        {
            foreach (var job in _jobs)
                job.Cancel();
        }
    }

    /// <summary>
    /// Pauses a specific job by its ID.
    /// </summary>
    /// <param name="jobId">The ID of the job to pause.</param>
    public void PauseJob(Guid jobId)
    {
        lock (_lock)
        {
            var job = _jobs.FirstOrDefault(j => j.JobId == jobId);
            job?.Pause();
        }
    }

    /// <summary>
    /// Resumes a specific job by its ID.
    /// </summary>
    /// <param name="jobId">The ID of the job to resume.</param>
    public void ResumeJob(Guid jobId)
    {
        lock (_lock)
        {
            var job = _jobs.FirstOrDefault(j => j.JobId == jobId);
            job?.Resume();
        }
    }

    /// <summary>
    /// Retrieves all jobs along with their associated logs and results.
    /// </summary>
    /// <returns>An enumerable collection of job responses.</returns>
    public IEnumerable<JobResponse> GetAllJobsWithLogs()
    {
        lock (_lock)
        {
            foreach (var job in _jobs)
            {
                var logs = _logger.GetLogs(job.JobId.ToString());
                yield return new JobResponse
                {
                    Job = job,
                    Logs = logs,
                    Result = job.Result
                };
            }
        }
    }

    /// <summary>
    /// Determines whether there are any running jobs.
    /// </summary>
    /// <returns>True if there are running jobs; otherwise, false.</returns>
    public bool HasRunningJobs()
    {
        lock (_lock)
        {
            return _jobs.Any(j => j.Status == "Running");
        }
    }

    /// <summary>
    /// Cancels all running jobs and waits for them to complete or cancel.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task CancelOrWaitForRunningJobsAsync()
    {
        lock (_lock)
        {
            if (!_jobs.Any(j => j.Status == "Running"))
                return;

            foreach (var job in _jobs.Where(j => j.Status == "Running"))
                job.Cancel();
        }

        while (true)
        {
            lock (_lock)
            {
                if (!_jobs.Any(j => j.Status == "Running"))
                    return;
            }
            await Task.Delay(100);
        }
    }
}
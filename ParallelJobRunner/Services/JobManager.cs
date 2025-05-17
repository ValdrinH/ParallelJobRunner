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

    public void CancelAll()
    {
        lock (_lock)
        {
            foreach (var job in _jobs)
                job.Cancel();
        }
    }

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

    public bool HasRunningJobs()
    {
        lock (_lock)
        {
            return _jobs.Any(j => j.Status == "Running");
        }
    }

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
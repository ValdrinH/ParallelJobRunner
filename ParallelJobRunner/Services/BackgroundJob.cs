using System.ComponentModel;
using ParallelJobRunner.Interfaces;

namespace ParallelJobRunner.Services
{
    /// <summary>
    /// Represents a background job that executes an asynchronous task with a specified return type.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the job.</typeparam>
    public class BackgroundJob<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the unique identifier for the job.
        /// </summary>
        public Guid JobId { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the name of the job.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the predicted duration of the job.
        /// </summary>
        public TimeSpan PredictedDuration { get; }

        /// <summary>
        /// Gets the result of the job after execution, if completed successfully.
        /// </summary>
        public T? Result { get; private set; }

        private string _status = "Pending";

        /// <summary>
        /// Gets or sets the current status of the job (e.g., Pending, Running, Paused, Completed, Cancelled, Error).
        /// </summary>
        public string Status
        {
            get => _status;
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                }
            }
        }

        /// <summary>
        /// Gets the start time of the job.
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Gets the end time of the job.
        /// </summary>
        public DateTime EndTime { get; private set; }

        /// <summary>
        /// Gets the duration of the job, calculated as the difference between end and start times.
        /// </summary>
        public TimeSpan Duration => EndTime - StartTime;

        /// <summary>
        /// Gets the asynchronous action that the job executes.
        /// </summary>
        public Func<CancellationToken, Task<T>> JobAction { get; }

        /// <summary>
        /// Gets the callback invoked when the job completes successfully.
        /// </summary>
        public Action<T>? OnCompleted { get; }

        /// <summary>
        /// Gets the callback invoked when the job fails with an exception.
        /// </summary>
        public Action<Exception>? OnError { get; }

        private readonly CancellationTokenSource _cts = new();
        private readonly IJobLogger _logger;
        private volatile bool _isPaused;
        private TaskCompletionSource<bool>? _resumeTcs;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundJob{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the job.</param>
        /// <param name="jobAction">The asynchronous action to execute.</param>
        /// <param name="predictedDuration">The predicted duration of the job.</param>
        /// <param name="onCompleted">The callback to invoke on successful completion.</param>
        /// <param name="onError">The callback to invoke on failure.</param>
        /// <param name="logger">The logger for job events. If null, uses <see cref="JobLoggerProvider.Instance"/>.</param>
        public BackgroundJob(string name, Func<CancellationToken, Task<T>> jobAction, TimeSpan predictedDuration,
            Action<T>? onCompleted = null, Action<Exception>? onError = null,
            IJobLogger? logger = null)
        {
            Name = name;
            JobAction = jobAction;
            PredictedDuration = predictedDuration;
            OnCompleted = onCompleted;
            OnError = onError;
            _logger = logger ?? JobLoggerProvider.Instance;
        }

        /// <summary>
        /// Executes the job asynchronously and handles its lifecycle.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunAsync()
        {
            try
            {
                _logger.Log(JobId.ToString(), $"[START] Job '{Name}' started.");
                Status = "Running";
                StartTime = DateTime.Now;

                // Execute and await the job action
                Result = await JobAction(_cts.Token);

                EndTime = DateTime.Now;
                Status = "Completed";
                _logger.Log(JobId.ToString(), $"[SUCCESS] Job '{Name}' completed in {Duration.TotalSeconds:F2}s with result: {Result}");
                OnCompleted?.Invoke(Result);
            }
            catch (OperationCanceledException)
            {
                EndTime = DateTime.Now;
                Status = "Cancelled";
                _logger.Log(JobId.ToString(), $"[CANCELLED] Job '{Name}' was cancelled after {Duration.TotalSeconds:F2}s.");
            }
            catch (Exception ex)
            {
                EndTime = DateTime.Now;
                Status = "Error";
                _logger.Log(JobId.ToString(), $"[ERROR] Job '{Name}' failed: {ex.Message}");
                OnError?.Invoke(ex);
            }
            finally
            {
                _cts.Dispose();
            }
        }

        /// <summary>
        /// Cancels the job if it is running or paused.
        /// </summary>
        public void Cancel()
        {
            _cts.Cancel();
            if (_isPaused)
            {
                _resumeTcs?.SetResult(true); // Unblock any awaiting CheckPauseAsync calls
                _resumeTcs = null;
            }
            _isPaused = false;
            _logger.Log(JobId.ToString(), $"[REQUEST] Cancel requested for job '{Name}'.");
        }

        /// <summary>
        /// Pauses the job if it is running.
        /// </summary>
        public void Pause()
        {
            if (Status == "Running")
            {
                _isPaused = true;
                _resumeTcs = new TaskCompletionSource<bool>();
                Status = "Paused";
                _logger.Log(JobId.ToString(), $"[PAUSED] Job '{Name}' paused.");
            }
        }

        /// <summary>
        /// Resumes the job if it is paused.
        /// </summary>
        public void Resume()
        {
            if (Status == "Paused")
            {
                _isPaused = false;
                _resumeTcs?.SetResult(true);
                _resumeTcs = null;
                Status = "Running";
                _logger.Log(JobId.ToString(), $"[RESUMED] Job '{Name}' resumed.");
            }
        }

        /// <summary>
        /// Checks if the job is paused and waits until resumed or cancelled.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A task that completes when the job is not paused or cancellation is requested.</returns>
        public async Task CheckPauseAsync(CancellationToken ct)
        {
            while (_isPaused)
            {
                if (_resumeTcs == null)
                    break;

                var resumeTask = _resumeTcs.Task;
                var cancelTask = Task.Delay(-1, ct);
                var completedTask = await Task.WhenAny(resumeTask, cancelTask);

                if (completedTask == cancelTask)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
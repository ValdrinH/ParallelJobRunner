using ParallelJobRunner.Interfaces;

namespace ParallelJobRunner.Services
{
    public class BackgroundJobWrapper<T> : IBackgroundJobWrapper
    {
        public BackgroundJob<T> Job { get; }
        public BackgroundJobWrapper(BackgroundJob<T> job) => Job = job;

        public Guid JobId => Job.JobId;
        public string Name => Job.Name;
        public string Status => Job.Status;

        public object? Result => Job.Result;

        public void Cancel() => Job.Cancel();

        public override string ToString() => $"{Name} - {Status}";
    }
}

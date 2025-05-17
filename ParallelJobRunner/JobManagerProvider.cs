namespace ParallelJobRunner
{
    public static class JobManagerProvider
    {
        private static readonly Lazy<JobManager> _instance = new(() => new JobManager());
        public static JobManager Instance => _instance.Value;
    }
}

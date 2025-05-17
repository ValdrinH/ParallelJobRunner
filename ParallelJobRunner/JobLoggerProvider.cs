using ParallelJobRunner.Interfaces;
using ParallelJobRunner.Services;

namespace ParallelJobRunner
{
    public static class JobLoggerProvider
    {
        private static readonly Lazy<IJobLogger> _instance = new(() => new JobLogger());
        public static IJobLogger Instance => _instance.Value;
    }
}

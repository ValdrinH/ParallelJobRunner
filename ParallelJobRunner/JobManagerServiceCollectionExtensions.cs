using Microsoft.Extensions.DependencyInjection;
using ParallelJobRunner.Interfaces;
using ParallelJobRunner.Services;

namespace ParallelJobRunner
{
    public static class JobManagerServiceCollectionExtensions
    {
        public static IServiceCollection AddJobManager(this IServiceCollection services)
        {
            return services.AddSingleton<IJobManager, JobManager>();
        }
    }
}

using Hangfire;

namespace AccountServices
{
    public class ScopedJobActivator : JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public ScopedJobActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type jobType)
        {
            return ActivatorUtilities.CreateInstance(_serviceProvider, jobType);
        }
    }
}

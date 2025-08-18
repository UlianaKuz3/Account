using Hangfire;

namespace AccountServices
{
    // ReSharper disable once UnusedMember.Global
    public class ScopedJobActivator(IServiceProvider serviceProvider) : JobActivator
    {

        public override object ActivateJob(Type jobType)
        {
            return ActivatorUtilities.CreateInstance(serviceProvider, jobType);
        }
    }
}

using Hangfire;
using ScheduledTaskService.Interface;
using System.Linq.Expressions;



namespace ScheduledTaskService.Services
{
    public class JobTaskScheduler : IJobTaskScheduler
    {
        private readonly IRecurringJobManager _recurringJobManager;

        public JobTaskScheduler(IRecurringJobManager recurringJobManager)
        {
            _recurringJobManager = recurringJobManager;
        }

        // Common method to add or update a recurring job
        public void ScheduleRecurringJob(string jobId, Expression<Action> methodCall, string cronExpression)
        {
            _recurringJobManager.AddOrUpdate(jobId, methodCall, cronExpression);
        }
    }
}

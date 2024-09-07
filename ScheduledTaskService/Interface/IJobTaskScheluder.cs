using System.Linq.Expressions;

namespace ScheduledTaskService.Interface
{
    public interface IJobTaskScheduler
    {
        void ScheduleRecurringJob(string jobId, Expression<Action> methodCall, string cronExpression);
    }
}

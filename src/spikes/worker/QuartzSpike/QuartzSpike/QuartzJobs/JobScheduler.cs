using Quartz;
using Quartz.Impl;

namespace QuartzSpike.QuartzJobs
{
	public class JobScheduler
	{
		public static void Start()
		{
			var scheduler = StdSchedulerFactory.GetDefaultScheduler();
			scheduler.Start();

			var job = JobBuilder.Create<UpdateFileJob>().Build();

			var trigger = TriggerBuilder.Create()
				.StartNow()
				.WithSimpleSchedule(x => x.WithIntervalInSeconds(5).RepeatForever())
				.Build();
			scheduler.ScheduleJob(job, trigger);
		}
	}
}
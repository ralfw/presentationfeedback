using Contract;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;

namespace nsapp.body.providers
{
	public class SchedulingProvider : ISchedulingProvider
	{
		private readonly IScheduler scheduler;

		public SchedulingProvider()
		{
			scheduler = StdSchedulerFactory.GetDefaultScheduler();
		}


		public void Start(int schedulerRepeatInterval, Action action) {
			var job = new Job(action);
			var jobFactory = new JobFactory(job);
			var jobDetails = JobBuilder.Create<Job>().Build();
			var trigger = TriggerBuilder.Create()
				.StartNow()
				.WithSimpleSchedule(x => x.WithIntervalInMinutes(schedulerRepeatInterval).RepeatForever())
				.Build();
			scheduler.JobFactory = jobFactory;
			scheduler.Start();
			scheduler.ScheduleJob(jobDetails, trigger);
		}

		public void Stop() {
			if (scheduler.IsStarted) {
				scheduler.Shutdown();
			}
		}


		private class Job : IJob {
			private readonly Action action;

			public Job(Action action) {
				this.action = action;
			}

			public void Execute(IJobExecutionContext context) {
				action();
			}
		}

		private class JobFactory : IJobFactory {
			private readonly IJob job;

			public JobFactory(IJob job)	{
				this.job = job;
			}

			public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)	{
				return job;
			}

			public void ReturnJob(IJob j) {
				throw new System.NotImplementedException();
			}
		}
	}
}
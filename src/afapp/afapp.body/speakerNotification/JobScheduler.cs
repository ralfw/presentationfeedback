using Quartz;
using Quartz.Impl;

namespace afapp.body.speakerNotification
{
	public class JobScheduler
	{
		public static void Start(int interval)
		{
			var scheduler = StdSchedulerFactory.GetDefaultScheduler();
			scheduler.Start();

//			var job = new JobDetailImpl(new SpeakerNotificationJob(null));
//			var job = JobBuilder.Create<SpeakerNotificationJob>().Build();

			var trigger = TriggerBuilder.Create()
				.StartNow()
				.WithSimpleSchedule(x => x.WithIntervalInMinutes(interval).RepeatForever())
				.Build();
			scheduler.ScheduleJob(job, trigger);
		}
	}
}

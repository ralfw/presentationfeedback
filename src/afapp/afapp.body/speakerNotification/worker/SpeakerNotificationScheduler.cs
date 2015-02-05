using Quartz;
using Quartz.Impl;

namespace afapp.body.speakerNotification.worker
{
	internal class SpeakerNotificationScheduler
	{
		private readonly IScheduler scheduler;
		private readonly IEmailService emailService;
		private readonly INotificationDataProvider dataProvider;
		private readonly Mapper mapper;

		public SpeakerNotificationScheduler(IEmailService emailService, INotificationDataProvider dataProvider, Mapper mapper)
		{
			scheduler = StdSchedulerFactory.GetDefaultScheduler();
			this.emailService = emailService;
			this.dataProvider = dataProvider;
			this.mapper = mapper;
		}

		internal void Start(int feedbackPeriod, int schedulerRepeatInterval)
		{
			var job = new SpeakerNotificationJob(emailService, dataProvider, mapper, feedbackPeriod, schedulerRepeatInterval);
			scheduler.JobFactory = new SpeakerNotificationJobFactory(job);
			scheduler.Start();

			var jobDetails = JobBuilder.Create<SpeakerNotificationJob>().Build();
			var trigger = TriggerBuilder.Create()
				.StartNow()
				.WithSimpleSchedule(x => x.WithIntervalInSeconds(schedulerRepeatInterval).RepeatForever())
				.Build();

			scheduler.ScheduleJob(jobDetails, trigger);
		}

		internal void Stop()
		{
			if (scheduler.IsStarted)
			{
				scheduler.Shutdown();
			}
		}
	}
}

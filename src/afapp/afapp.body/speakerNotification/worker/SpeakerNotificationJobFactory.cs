
using Quartz;
using Quartz.Spi;

namespace afapp.body.speakerNotification.worker
{
	internal class SpeakerNotificationJobFactory : IJobFactory
	{
		private readonly SpeakerNotificationJob job;

		public SpeakerNotificationJobFactory(SpeakerNotificationJob job)
		{
			this.job = job;
		}

		public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
		{
			return job;
		}

		public void ReturnJob(IJob j)
		{
			throw new System.NotImplementedException();
		}
	}
}

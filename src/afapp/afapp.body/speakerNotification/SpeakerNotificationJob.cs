using Quartz;

namespace afapp.body.speakerNotification
{
	internal class SpeakerNotificationJob : IJob
	{
		private readonly SpeakerNotificationHandler handler;

		public SpeakerNotificationJob(SpeakerNotificationHandler handler)
		{
			this.handler = handler;
		}

		public void Execute(IJobExecutionContext context)
		{
			handler.Run();
		}
	}
}

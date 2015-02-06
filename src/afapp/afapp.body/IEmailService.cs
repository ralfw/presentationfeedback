using afapp.body.data;

namespace afapp.body
{
	public interface IEmailService
	{
		void Send_speaker_notification(SpeakerNotificationData data);
	}
}

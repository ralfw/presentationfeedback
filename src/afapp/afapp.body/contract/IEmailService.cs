using afapp.body.data;

namespace afapp.body.contract
{
	public interface IEmailService
	{
		void Notify_speaker(SpeakerNotificationData data);
	}
}

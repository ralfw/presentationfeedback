using afapp.body.data;

namespace afapp.body.contract
{
	public interface INotificationProvider
	{
		void Send_feedback(SpeakerNotificationData data);
	}
}

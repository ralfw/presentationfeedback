
using Contract.data;

namespace Contract
{
	public interface INotificationProvider
	{
		void Send_feedback(SpeakerNotificationData data);
	}
}

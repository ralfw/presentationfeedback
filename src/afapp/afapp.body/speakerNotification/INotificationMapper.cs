using afapp.body.data;
using afapp.body.data.contract;
using System.Collections.Generic;

namespace afapp.body.speakerNotification
{
	public interface INotificationMapper
	{
		SpeakerNotificationData Map(string confTitle, SessionData session, IEnumerable<TrafficLightScores> scores);
	}
}

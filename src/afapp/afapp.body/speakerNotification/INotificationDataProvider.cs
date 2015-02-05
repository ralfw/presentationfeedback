using afapp.body.data;
using afapp.body.data.contract;
using System.Collections.Generic;

namespace afapp.body.speakerNotification
{
	public interface INotificationDataProvider
	{
		IEnumerable<SessionData> Get_all_sessions();

		string Get_conference_title(string sessionId);

		IEnumerable<TrafficLightScores> Get_scores(string sessionId);
	}
}

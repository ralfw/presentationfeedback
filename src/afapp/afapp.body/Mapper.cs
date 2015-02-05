using afapp.body.data;
using afapp.body.data.contract;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body
{
	public class Mapper {
		public SessionOverview Map(string confId, string confTitle, IEnumerable<ConferenceData.SessionData> activeSessions, IEnumerable<ConferenceData.SessionData> inactiveSessions) {
			return new SessionOverview { 
				ConfId = confId,
				ConfTitle = confTitle,

				ActiveSessions = activeSessions.Select(s => new SessionOverview.Session{
					Id = s.Id,
					Title = s.Title,
					Start = s.Start,
					End = s.End,
					SpeakerName = s.SpeakerName
				}).ToArray(),

				InactiveSessions = inactiveSessions.Select(s => new SessionOverview.Session{
					Id = s.Id,
					Title = s.Title,
					Start = s.Start,
					End = s.End,
					SpeakerName = s.SpeakerName
				}).ToArray(),
			};
		}

		public SpeakerNotificationData Map(string confTitle, ConferenceData.SessionData session, IEnumerable<TrafficLightScores> scores)
		{
			return new SpeakerNotificationData
			{
				ConferenceTitle = confTitle,
				Session = session,
				Reds = scores.Count(x => x == TrafficLightScores.Red),
				Yellows = scores.Count(x => x == TrafficLightScores.Yellow),
				Greens = scores.Count(x => x == TrafficLightScores.Green)
			};
		}
	}
}

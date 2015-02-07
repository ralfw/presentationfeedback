using afapp.body.contract.data;
using afapp.body.data;
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

		public SpeakerNotificationData Map(SessionWithScoresData session)
		{
			var scores = session.Scores(session.Id).ToList();
			return new SpeakerNotificationData
			{
				ConferenceTitle = session.ConferenceTitle(session.Id),
				Title = session.Title,
				Start = session.Start,
				End = session.End,
				SpeakerName = session.SpeakerName,
				SpeakerEmail = session.SpeakerEmail,
				Reds = scores.Count(x => x == TrafficLightScores.Red),
				Yellows = scores.Count(x => x == TrafficLightScores.Yellow),
				Greens = scores.Count(x => x == TrafficLightScores.Green)
			};
		}
	}
}

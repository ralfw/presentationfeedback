using Contract.data;
using Repository.data;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body
{
	public class Mapper {
		public SessionOverview Map(string confId, string confTitle, IEnumerable<ConferenceData.SessionData> activeSessions, 
			IEnumerable<ConferenceData.SessionData> inactiveSessions) {
			return new SessionOverview { 
				ConfId = confId,
				ConfTitle = confTitle,

				ActiveSessions = activeSessions.Select(s => new Session{
					Id = s.Id,
					Title = s.Title,
					Start = s.Start,
					End = s.End,
					SpeakerName = s.SpeakerName
				}).ToArray(),

				InactiveSessions = inactiveSessions.Select(s => new Session{
					Id = s.Id,
					Title = s.Title,
					Start = s.Start,
					End = s.End,
					SpeakerName = s.SpeakerName
				}).ToArray(),
			};
		}

		public SpeakerNotificationData Map(ScoredSessionData session)
		{
			var feedback = session.UniqueFeedback.ToList();
			return new SpeakerNotificationData
			{
				ConfTitle = session.ConfTitle,
				Title = session.Title,
				Start = session.Start,
				End = session.End,
				SpeakerName = session.SpeakerName,
				SpeakerEmail = session.SpeakerEmail,
				Reds = feedback.Count(x => x.Score == TrafficLightScores.Red),
				Yellows = feedback.Count(x => x.Score == TrafficLightScores.Yellow),
				Greens = feedback.Count(x => x.Score == TrafficLightScores.Green)
			};
		}
	}
}

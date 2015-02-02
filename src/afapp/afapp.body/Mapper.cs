using System;
using EventStore.Contract;
using System.Collections.Generic;
using System.Linq;
using afapp.body.data.contract;
using afapp.body.data;

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
	}
}

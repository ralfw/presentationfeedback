using System;
using EventStore.Contract;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body
{

	public class Mapper {
		public SessionOverviewVM Map(string confId, string confTitle, IEnumerable<SessionData> activeSessions, IEnumerable<SessionData> inactiveSessions) {
			return new SessionOverviewVM { 
				ConfId = confId,
				ConfTitle = confTitle,

				ActiveSessions = activeSessions.Select(s => new SessionVM{
					Id = s.Id,
					Title = s.Title,
					Start = s.Start,
					End = s.End,
					SpeakerName = s.SpeakerName
				}).ToArray(),

				InactiveSessions = inactiveSessions.Select(s => new SessionVM{
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

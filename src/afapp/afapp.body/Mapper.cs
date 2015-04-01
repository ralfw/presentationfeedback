
using Contract.data;
using Repository.data;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body
{
	using System;

	public class Mapper
	{
		public SessionOverview Map(string confId, string confTitle, string timeZoneString, IEnumerable<ConferenceData.SessionData> activeSessions,
								   IEnumerable<ConferenceData.SessionData> inactiveSessions)
		{
			return new SessionOverview
			{
				ConfId = confId,
				ConfTitle = confTitle,
				TimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString),

				ActiveSessions = activeSessions.Select(s => new Session
				{
					Id = s.Id,
					Title = s.Title,
					Start = s.Start,
					End = s.End,
					SpeakerName = s.SpeakerName
				}).ToArray(),

				InactiveSessions = inactiveSessions.Select(s => new Session
				{
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

using System;
using EventStore.Contract;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body
{
	//TODO: What about time zones? This might be checked on a server in a different timezone than the conference.
	public class Conference {
		private const int ACTIVE_SESSION_BUFFER_MIN = 20;
		private ConferenceData confdata;
		private Func<DateTime> now;

		public Conference(ConferenceData confdata, Func<DateTime> now) {
			this.confdata = confdata;
			this.now = now;
		}

		public IEnumerable<SessionData> DetermineActiveSessions {get{ 
				return this.confdata.Sessions.Where (s => s.Start.AddMinutes (-ACTIVE_SESSION_BUFFER_MIN) <= now() &&
														  now() <= s.End.AddMinutes (ACTIVE_SESSION_BUFFER_MIN));
		}}

		public IEnumerable<SessionData> DetermineInactiveSessions {get{ 
				return this.confdata.Sessions.Where (s => now() < s.Start.AddMinutes (-ACTIVE_SESSION_BUFFER_MIN) ||
														  s.End.AddMinutes (ACTIVE_SESSION_BUFFER_MIN) < now());
		}}
	}

}

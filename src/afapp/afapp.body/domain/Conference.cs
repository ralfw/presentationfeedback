using System.Collections.Generic;
using System.Linq;
using afapp.body.data;

namespace afapp.body.domain
{
	//TODO: What about time zones? This might be checked on a server in a different timezone than the conference.
	public class Conference {
		private const int ACTIVE_SESSION_BUFFER_MIN = 20;
		private readonly ConferenceData confdata;

		public Conference(ConferenceData confdata) {
			this.confdata = confdata;
		}

		public IEnumerable<SessionData> DetermineActiveSessions {get{ 
				return this.confdata.Sessions.Where (s => s.Start.AddMinutes (-ACTIVE_SESSION_BUFFER_MIN) <= TimeProvider.Now() &&
													 TimeProvider.Now() <= s.End.AddMinutes (ACTIVE_SESSION_BUFFER_MIN));
		}}

		public IEnumerable<SessionData> DetermineInactiveSessions {get{ 
				return this.confdata.Sessions.Where (s => TimeProvider.Now() < s.Start.AddMinutes (-ACTIVE_SESSION_BUFFER_MIN) ||
													 s.End.AddMinutes (ACTIVE_SESSION_BUFFER_MIN) < TimeProvider.Now());
		}}
	}
}

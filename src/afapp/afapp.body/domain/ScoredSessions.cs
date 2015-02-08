using System;

namespace afapp.body.domain
{
	using data;
	using providers;
	using System.Collections.Generic;
	using System.Linq;

	public class XXX
	{
		private readonly IEnumerable<ScoredSessionData> sessions;


		public XXX(IEnumerable<ScoredSessionData> sessions)
		{
			this.sessions = sessions;
		}


		public IEnumerable<ScoredSessionData> Get_sessions_due_for_notification(int feedbackPeriod)
		{
			return sessions.Where(x => !x.SpeakerNotified &&
									   TimeProvider.Now() > x.End.AddMinutes(feedbackPeriod));
		}
	}
}

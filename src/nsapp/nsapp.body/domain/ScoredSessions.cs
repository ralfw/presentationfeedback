﻿using Contract.provider;
using Repository.data;
using System.Collections.Generic;
using System.Linq;

namespace nsapp.body.domain
{
	public class ScoredSessions
	{
		private readonly IEnumerable<ScoredSessionData> sessions;


		public ScoredSessions(IEnumerable<ScoredSessionData> sessions)
		{
			this.sessions = sessions;
		}


		public IEnumerable<ScoredSessionData> Get_sessions_due_for_notification(int feedbackPeriod)
		{
			return sessions.Where(x => !x.SpeakerNotified &&
									   TimeProvider.Now() > x.End.UtcTime.AddMinutes(feedbackPeriod));
		}
	}
}

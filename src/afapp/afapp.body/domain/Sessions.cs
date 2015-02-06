
namespace afapp.body.domain
{
	using data;
	using providers;
	using System.Collections.Generic;
	using System.Linq;

	public class Sessions
	{
		private readonly IEnumerable<SessionWithScoresData> sessions;

		public Sessions(IEnumerable<SessionWithScoresData> sessions)
		{
			this.sessions = sessions;
		}

		public IEnumerable<SessionWithScoresData> Get_sessions_due_for_notification(int feedbackPeriod)
		{
			return sessions.Where(x => TimeProvider.Now() > x.End.AddMinutes(feedbackPeriod) && !x.IsSpeakerNotifiedAboutSessionfeedback);
		}
	}
}

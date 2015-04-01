using Contract.provider;
using Repository.data;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body.domain
{
	using Contract.data;

	public class Conference {
		private readonly int feedbackPeriod;
		private readonly ConferenceData confdata;

		public Conference(ConferenceData confdata, int feedbackPeriod)
		{
			this.confdata = confdata;
			this.feedbackPeriod = feedbackPeriod;
		}

		public IEnumerable<ConferenceData.SessionData> DetermineActiveSessions {get{ 
				return this.confdata.Sessions.Where (s => s.Start.UtcTime.AddMinutes (-feedbackPeriod) <= TimeProvider.Now() &&
													 TimeProvider.Now() <= s.End.UtcTime.AddMinutes(feedbackPeriod))
											.OrderBy(x => x.Start.UtcTime);
		}}

		public IEnumerable<ConferenceData.SessionData> DetermineInactiveSessions {get{ 
				return this.confdata.Sessions.Where (s => TimeProvider.Now() < s.Start.UtcTime.AddMinutes (-feedbackPeriod) ||
													 s.End.UtcTime.AddMinutes (feedbackPeriod) < TimeProvider.Now())
											.OrderBy(x => x.Start.UtcTime);
		}}

		public DateTimeWithZone StartDate {get { return confdata.Sessions.OrderBy(x => x.Start.UtcTime).First().Start; }}

		public DateTimeWithZone EndDate { get { return confdata.Sessions.OrderBy(x => x.End.UtcTime).Last().End; } }
	}
}

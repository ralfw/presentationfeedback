using EventStore.Contract;
using System;

namespace afapp.body
{
	public class Body
	{
		private readonly Repository repo;
		private readonly Func<DateTime> now;

		public Body (IEventStore es, Func<DateTime> now) {
			this.repo = new Repository(es);
			this.now = now;
		} 
			
		public SessionOverviewVM GenerateSessionOverview(string confId) {
			var confdata = this.repo.LoadConference (confId);

			var conf = new Conference (confdata, this.now);
			var activeSessions = conf.DetermineActiveSessions;
			var inactiveSessions = conf.DetermineInactiveSessions;

			return Mapper.Map (confdata.Id, confdata.Title, activeSessions, inactiveSessions);
		}

		public void Store_feedback(FeedbackData data)
		{
			repo.Store_feedback(data);
		}
	}
}


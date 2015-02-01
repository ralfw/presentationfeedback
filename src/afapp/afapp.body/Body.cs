using EventStore.Contract;
using System;

namespace afapp.body
{
	public class Body
	{
		private readonly Repository repo;
		private Func<ConferenceData, Func<DateTime>, Conference> conferenceFactory;
		private Mapper mapper;
		private readonly Func<DateTime> now;

		public Body (Repository repo, Func<ConferenceData, Func<DateTime>, Conference> conferenceFactory, Mapper mapper, Func<DateTime> now) {
			this.repo = repo;
			this.conferenceFactory = conferenceFactory;
			this.mapper = mapper;
			this.now = now;
		} 
			
		public SessionOverviewVM GenerateSessionOverview(string confId) {
			var confdata = this.repo.LoadConference (confId);

			var conf = this.conferenceFactory (confdata, this.now);
			var activeSessions = conf.DetermineActiveSessions;
			var inactiveSessions = conf.DetermineInactiveSessions;

			return this.mapper.Map (confdata.Id, confdata.Title, activeSessions, inactiveSessions);
		}

		public void Store_feedback(FeedbackData data)
		{
			repo.Store_feedback(data);
		}
	}
}


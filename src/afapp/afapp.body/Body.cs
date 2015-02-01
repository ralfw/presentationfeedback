using EventStore.Contract;
using System;

namespace afapp.body
{
	public class Body
	{
		private readonly Repository repo;
		private Func<ConferenceData, Func<DateTime>, Conference> conferenceFactory;
		private Mapper mapper;

		public Body (Repository repo, Func<ConferenceData, Func<DateTime>, Conference> conferenceFactory, Mapper mapper) {
			this.repo = repo;
			this.conferenceFactory = conferenceFactory;
			this.mapper = mapper;
			this.Now = () => DateTime.Now;
		} 


		public Func<DateTime> Now { get; set;}
			

		public SessionOverviewVM GenerateSessionOverview(string confId) {
			var confdata = this.repo.LoadConference (confId);

			var conf = this.conferenceFactory (confdata, this.Now);
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


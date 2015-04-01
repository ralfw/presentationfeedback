using afapp.body.domain;
using Contract.data;
using Repository.data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body
{

	public class Body
	{
		private readonly Repository.Repository repo;
		private readonly Func<ConferenceData, Conference> conferenceFactory;
		private readonly Mapper mapper;

		public Body(Repository.Repository repo, Func<ConferenceData, Conference> conferenceFactory, Mapper mapper)
		{
			this.repo = repo;
			this.conferenceFactory = conferenceFactory;
			this.mapper = mapper;
		} 

		public SessionOverview Generate_session_overview(string confId) {
			var confdata = repo.Load_conference (confId);

			var conf = conferenceFactory (confdata);
			var activeSessions = conf.DetermineActiveSessions;
			var inactiveSessions = conf.DetermineInactiveSessions;

			return mapper.Map (confdata.Id, confdata.Title, confdata.TimeZone, activeSessions, inactiveSessions);
		}

		public void Store_feedback(string sessionId, TrafficLightScores score, string comment, string email)
		{
			//TODO: maybe a check if this is a valid session id would be nice
			//TODO: maybe a check if the session really is active would be nice
			repo.Register_feedback(new FeedbackData{
				SessionId = sessionId,
				Score = score,
				Comment = comment,
				Email = email
			});
		}

		public IEnumerable<ConferenceVm> Generate_conference_overview()
		{
			return repo.Load_conferences().Select(conferenceData =>
			{
				var conference = conferenceFactory(conferenceData);
				return new ConferenceVm
				{
					Id = conferenceData.Id,
					Title = conferenceData.Title,
					Start = conference.StartDate,
					End = conference.EndDate,
					TimeZone = TimeZoneInfo.FindSystemTimeZoneById(conferenceData.TimeZone)
				};
			});
		}

		public ConferenceData.SessionData Get_Session(string confid, string id)
		{
			return repo.Load_conference(confid).Sessions.Single(x => x.Id == id);
		}
	}
}
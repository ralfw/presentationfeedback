using afapp.body.contract;
using afapp.body.contract.data;
using afapp.body.data;
using afapp.body.domain;
using afapp.body.providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body
{
	public class Body
	{
		private readonly Repository repo;
		private readonly Func<ConferenceData, Conference> conferenceFactory;
		private readonly Func<IEnumerable<ScoredSessionData>, XXX> xxxFactory;
		private readonly Mapper mapper;
		private readonly SchedulingProvider scheduler;
		private readonly IEmailService emailService;

		public Body (Repository repo, Func<ConferenceData, Conference> conferenceFactory, Mapper mapper,
					 SchedulingProvider scheduler, IEmailService emailService,
					 Func<IEnumerable<ScoredSessionData>, XXX> xxxFactory)
		{
			this.repo = repo;
			this.conferenceFactory = conferenceFactory;
			this.mapper = mapper;
			this.scheduler = scheduler;
			this.emailService = emailService;
			this.xxxFactory = xxxFactory;
		} 

		public SessionOverview GenerateSessionOverview(string confId) {
			var confdata = this.repo.Load_conference (confId);

			var conf = this.conferenceFactory (confdata);
			var activeSessions = conf.DetermineActiveSessions;
			var inactiveSessions = conf.DetermineInactiveSessions;

			return this.mapper.Map (confdata.Id, confdata.Title, activeSessions, inactiveSessions);
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

		public void Start_background_speaker_notification(int feedbackPeriod, int schedulerRepeatInterval)
		{
			scheduler.Start(schedulerRepeatInterval,
			  () => {
					var scoredSessionsData = repo.Load_scored_sessions();
					var xxx = xxxFactory(scoredSessionsData);
				    var dueSessions = xxx.Get_sessions_due_for_notification(feedbackPeriod);
					dueSessions.ToList().ForEach(
						Notify_speaker);
			  });
		}

		private void Notify_speaker(ScoredSessionData sessionData)
		{
			var notificationData = mapper.Map(sessionData);
			emailService.Notify_speaker(notificationData);
			repo.Remember_speaker_got_notified_about_session_feedback(sessionData);
		}

		public void Stop_speaker_notification()
		{
			scheduler.Stop();
		}
	}
}
using afapp.body.contract;
using afapp.body.contract.data;
using afapp.body.data;
using afapp.body.domain;
using afapp.body.helpers;
using System;
using System.Collections.Generic;

namespace afapp.body
{
	public class Body
	{
		private readonly Repository repo;
		private readonly Func<ConferenceData, Conference> conferenceFactory;
		private readonly Func<IEnumerable<ScoredSessionData>, ScoredSessions> scoredSessionsFactory;
		private readonly Mapper mapper;
		private readonly ISchedulingProvider scheduler;
		private readonly INotificationProvider notifier;

		public Body (Repository repo, Func<ConferenceData, Conference> conferenceFactory, Mapper mapper,
					 ISchedulingProvider scheduler, INotificationProvider notifier,
					 Func<IEnumerable<ScoredSessionData>, ScoredSessions> scoredSessionsFactory)
		{
			this.repo = repo;
			this.conferenceFactory = conferenceFactory;
			this.mapper = mapper;
			this.scheduler = scheduler;
			this.notifier = notifier;
			this.scoredSessionsFactory = scoredSessionsFactory;
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
					var scoredSessions = scoredSessionsFactory(scoredSessionsData);
					scoredSessions.Get_sessions_due_for_notification(feedbackPeriod)
						.ForEach(Notify_speaker);
			  });
		}

		private void Notify_speaker(ScoredSessionData scoredSessionData)
		{
			var notificationData = mapper.Map(scoredSessionData);
			notifier.Send_feedback(notificationData);
			repo.Register_feedback_notification(scoredSessionData.Id);
		}


		public void Stop_speaker_notification()
		{
			scheduler.Stop();
		}
	}
}
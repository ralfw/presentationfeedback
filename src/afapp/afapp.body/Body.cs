﻿using pfapp.body.helpers;
using Contract;
using Contract.data;
using log4net;
using Repository.data;
using System;
using System.Collections.Generic;
using System.Linq;
using pfapp.body.domain;
using pfapp.body.providers;

namespace pfapp.body
{

	public class Body
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(Body));
		private readonly Repository.Repository repo;
		private readonly Func<ConferenceData, Conference> conferenceFactory;
		private readonly Func<IEnumerable<ScoredSessionData>, ScoredSessions> scoredSessionsFactory;
		private readonly Mapper mapper;
		private readonly ISchedulingProvider scheduler;
		private readonly INotificationProvider notifier;

		public Body(Repository.Repository repo, Func<ConferenceData, Conference> conferenceFactory, Mapper mapper,
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

		public SessionOverview Generate_session_overview(string confId) {
			var confdata = repo.Load_conference (confId);

			var conf = conferenceFactory (confdata);
			var activeSessions = conf.DetermineActiveSessions;
			var inactiveSessions = conf.DetermineInactiveSessions;

			return mapper.Map (confdata.Id, confdata.Title, activeSessions, inactiveSessions);
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
					Logger.Info(string.Format("Notification scheduler runs: {0} - feedbackPeriod: {1} - schedulerRepeatInterval: {2}", 
						TimeProvider.Now, feedbackPeriod, schedulerRepeatInterval));
					var scoredSessionsData = repo.Load_scored_sessions();
					var scoredSessions = scoredSessionsFactory(scoredSessionsData);
					scoredSessions.Get_sessions_due_for_notification(feedbackPeriod)
						.ForEach(Notify_speaker);
			  });
		}

		private void Notify_speaker(ScoredSessionData scoredSessionData)
		{
			Logger.Info(string.Format("Notify speaker {0} about session {1}", scoredSessionData.SpeakerEmail, scoredSessionData.Title));
			var notificationData = mapper.Map(scoredSessionData);
			notifier.Send_feedback(notificationData);
			repo.Register_feedback_notification(scoredSessionData.Id);
		}

		public void Stop_speaker_notification()
		{
			scheduler.Stop();
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
					End = conference.EndDate
				};
			});
		}

		public Session Get_Session(string sessionId)
		{
			return repo.Get_Session(sessionId);
		}
	}
}
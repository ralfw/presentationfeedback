using afapp.body.data;
using afapp.body.data.contract;
using afapp.body.domain;
using afapp.body.speakerNotification;
using afapp.body.speakerNotification.worker;
using System;

namespace afapp.body
{
	public class Body
	{
		private readonly Repository repo;
		private readonly Func<ConferenceData, Conference> conferenceFactory;
		private readonly Mapper mapper;
		private readonly SpeakerNotificationScheduler scheduler;

		public Body (Repository repo, Func<ConferenceData, Conference> conferenceFactory, Mapper mapper,
					 IEmailService emailService) {
			this.repo = repo;
			this.conferenceFactory = conferenceFactory;
			this.mapper = mapper;
			scheduler = new SpeakerNotificationScheduler(emailService, repo, mapper);
		} 

		public SessionOverview GenerateSessionOverview(string confId) {
			var confdata = this.repo.LoadConference (confId);

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


		public void Start_speaker_notification_scheduler(int feedbackPeriod, int schedulerRepeatInterval)
		{
			scheduler.Start(feedbackPeriod, schedulerRepeatInterval);
		}

		public void Stop_speaker_notification_scheduler()
		{
			scheduler.Stop();
		}
	}
}
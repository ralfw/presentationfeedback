using afapp.body.data;
using afapp.body.contract.data;
using afapp.body.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using afapp.body.providers;
using afapp.body.contract;

namespace afapp.body
{
	public class Body
	{
		private readonly Repository repo;
		private readonly Func<ConferenceData, Conference> conferenceFactory;
		private readonly Mapper mapper;
		private readonly SchedulingProvider scheduler;
		private readonly IEmailService emailService;

		public Body (Repository repo, Func<ConferenceData, Conference> conferenceFactory, Mapper mapper,
					 SchedulingProvider scheduler, IEmailService emailService) {
			this.repo = repo;
			this.conferenceFactory = conferenceFactory;
			this.mapper = mapper;
			this.scheduler = scheduler;
			this.emailService = emailService;
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


		public void Start_background_speaker_notification(int feedbackPeriod, int schedulerRepeatInterval)
		{
			scheduler.Start(schedulerRepeatInterval,
			  () => {
				  var sessions = Find_sessions_due_for_notification(feedbackPeriod);
				  sessions.ToList().ForEach(Notify_speaker);
			  });
		}

		//TODO: data handling seems to be complicated
		private IEnumerable<ConferenceData.SessionData> Find_sessions_due_for_notification(int feedbackPeriod)
		{
			var sessions = repo.Get_all_sessions();
			//TODO: this should be a matter of the Conference I guess; Sessions could be marked as closed after speaker got notified
			return sessions.Where(x => TimeProvider.Now() > x.End.AddMinutes(feedbackPeriod)
								  && repo.Get_handled_session_ids().All(y => y != x.Id));
		}

		private void Notify_speaker(ConferenceData.SessionData session)
		{
			var notification = Build_speaker_feedback_notification(session);
			emailService.Notify_speaker(notification);
			repo.Remember_speaker_got_notified_about_session_feedback(session);
		}

		private SpeakerNotificationData Build_speaker_feedback_notification(ConferenceData.SessionData sessionData)
		{
			var conferenceTitle = repo.Get_conference_title(sessionData.Id);
			var scores = repo.Get_scores(sessionData.Id);
			return mapper.Map(conferenceTitle, sessionData, scores);
		}


		public void Stop_speaker_notification()
		{
			scheduler.Stop();
		}
	}
}
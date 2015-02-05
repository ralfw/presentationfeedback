using afapp.body.data;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body.speakerNotification
{
	public class SpeakerNotificationJob : IJob
	{
		private readonly int feedbackPeriod;
		private readonly int schedulerRepeatInterval;
		private readonly IEmailService emailService;
		private readonly INotificationDataProvider dataProvider;
		private readonly Mapper mapper;

		public SpeakerNotificationJob(IEmailService emailService, INotificationDataProvider dataProvider,
			Mapper mapper, int feedbackPeriod, int schedulerRepeatInterval)
		{
			this.emailService = emailService;
			this.dataProvider = dataProvider;
			this.mapper = mapper;
			this.feedbackPeriod = feedbackPeriod;
			this.schedulerRepeatInterval = schedulerRepeatInterval;
		}

		public void Execute(IJobExecutionContext context)
		{
			var sessions = Find_sessions_due_for_notification();
			Process_sessions(sessions, Process_session);
		}

		private IEnumerable<SessionData> Find_sessions_due_for_notification()
		{
			var sessions = dataProvider.Get_all_sessions();
			return sessions.Where(x => TimeProvider.Now() > x.End.AddMinutes(feedbackPeriod) &&
			                    TimeProvider.Now() < x.End.AddMinutes(feedbackPeriod + schedulerRepeatInterval));
		}

		private static void Process_sessions(IEnumerable<SessionData> sessions, Action<SessionData> onSession)
		{
			foreach (var sessionData in sessions)
			{
				onSession(sessionData);
			}
		}

		private void Process_session(SessionData session)
		{
			var notification = BuildSpeakerNotification(session);
			emailService.Send_speaker_notification(notification);
		}

		private SpeakerNotificationData BuildSpeakerNotification(SessionData sessionData)
		{
			var conferenceTitle = dataProvider.Get_conference_title(sessionData.Id);
			var scores = dataProvider.Get_scores(sessionData.Id);
			return mapper.Map(conferenceTitle, sessionData, scores);
		}
	}
}

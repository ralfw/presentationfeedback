﻿using afapp.body.data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body.speakerNotification
{
	public class SpeakerNotificationHandler
	{
		private readonly int feedbackPeriod;
		private readonly int workerInvocationInterval;
		private readonly IEmailService emailService;
		private readonly INotificationDataProvider dataProvider;
		private readonly Mapper mapper;

		public SpeakerNotificationHandler(IEmailService emailService, INotificationDataProvider dataProvider,
			Mapper mapper, int feedbackPeriod, int workerInvocationInterval)
		{
			this.emailService = emailService;
			this.dataProvider = dataProvider;
			this.mapper = mapper;
			this.feedbackPeriod = feedbackPeriod;
			this.workerInvocationInterval = workerInvocationInterval;
		}

		public void Run()
		{
			var sessions = Find_sessions_due_for_notification();
			Process_sessions(sessions, Process_session);
		}

		private IEnumerable<SessionData> Find_sessions_due_for_notification()
		{
			var sessions = dataProvider.GetAllSessions();
			return sessions.Where(x => TimeProvider.Now().AddMinutes(feedbackPeriod) >= x.End &&
			                    TimeProvider.Now() < x.End.AddMinutes(feedbackPeriod + workerInvocationInterval));  
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

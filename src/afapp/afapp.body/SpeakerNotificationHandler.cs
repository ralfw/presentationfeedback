﻿
using afapp.body.data;
using afapp.body.data.contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body
{
	internal class SpeakerNotificationHandler
	{
		private const int FEEDBACK_PERIOD = 20;
		private readonly int workerInvocationInterval;
		private readonly IEmailService emailService;
		private readonly Repository repository;
		private readonly Mapper mapper;

		public SpeakerNotificationHandler(IEmailService emailService, Repository repository, Mapper mapper,
			int workerInvocationInterval)
		{
			this.emailService = emailService;
			this.repository = repository;
			this.mapper = mapper;
			this.workerInvocationInterval = workerInvocationInterval;
		}

		internal void Run()
		{
			var sessions = Find_sessions_due_for_notification();
			Process_sessions(sessions, Process_session);
		}

		private IEnumerable<SessionData> Find_sessions_due_for_notification()
		{
			var sessions = repository.GetAllSessions();
			return sessions.Where(x => TimeProvider.Now().AddMinutes(FEEDBACK_PERIOD) >= x.End &&
			                    TimeProvider.Now() < x.End.AddMinutes(FEEDBACK_PERIOD + workerInvocationInterval));  
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
			var conferenceTitle = repository.Get_conference_title(sessionData.Id);
			var scores = repository.Get_scores(sessionData.Id);
			return mapper.Map(conferenceTitle, sessionData, scores);
		}
	}
}


using Common.Logging;
using Contract;
using Contract.provider;
using nsapp.body.domain;
using nsapp.body.helpers;
using Repository.data;
using System;
using System.Collections.Generic;

namespace nsapp.body
{
	public class Body
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(Body));
		private readonly Repository.Repository repo;
		private readonly Func<IEnumerable<ScoredSessionData>, ScoredSessions> scoredSessionsFactory;
		private readonly Mapper mapper;
		private readonly ISchedulingProvider scheduler;
		private readonly INotificationProvider notifier;

		public Body(Repository.Repository repo, Mapper mapper, ISchedulingProvider scheduler, INotificationProvider notifier,
					 Func<IEnumerable<ScoredSessionData>, ScoredSessions> scoredSessionsFactory)
		{
			this.repo = repo;
			this.mapper = mapper;
			this.scheduler = scheduler;
			this.notifier = notifier;
			this.scoredSessionsFactory = scoredSessionsFactory;
		} 

		public void Start_background_speaker_notification(int feedbackPeriod, int schedulerRepeatInterval)
		{
			scheduler.Start(schedulerRepeatInterval,
			  () =>
			  {
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
	}
}

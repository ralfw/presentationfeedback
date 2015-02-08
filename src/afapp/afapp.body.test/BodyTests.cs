using afapp.body.contract;
using afapp.body.data;
using afapp.body.domain;
using afapp.body.providers;
using EventStore.Internals;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using EventStore.Internals;
using afapp.body.providers;
using System.Linq;


namespace afapp.body.test
{
	[TestFixture]
	class BodyTests
	{
		[Test]
		public void Speaker_notification() {
			// arrange
			var es = new EventStore.InMemoryEventStore ();
			var repo = new Repository (es);
			var map = new Mapper ();
			var fakeScheduler = new FakeSchedulingProvider ();
			var fakeNotifier = new FakeNotificationProvider ();
			var scoredSessionsFactory = new Func<IEnumerable<ScoredSessionData>, ScoredSessions> ((data) => new ScoredSessions(data));

			var body = new Body (repo, null, map, fakeScheduler, fakeNotifier, scoredSessionsFactory);

			// It would have been nice to have the coapp Repository{} available.
			es.Record (new Event ("c1", "ConferenceRegistered", "conf1"));
			es.Record (new Event ("c1s1", "SessionRegistered", "sess11\t2015-02-08T09:00:00\t2015-02-08T10:00:00\tname1\tname1@gmail.com"));
			es.Record (new Event("c1", "SessionAssigned", "c1s1"));
			es.Record (new Event ("c1s2", "SessionRegistered", "sess12\t2015-02-08T10:00:00\t2015-02-08T11:00:00\tname2\tname2@gmail.com"));
			es.Record (new Event("c1", "SessionAssigned", "c1s2"));
			es.Record (new Event ("c1s3", "SessionRegistered", "sess13\t2015-02-08T11:00:00\t2015-02-08T12:00:00\tname1\tname1@gmail.com"));
			es.Record (new Event("c1", "SessionAssigned", "c1s3"));

			es.Record (new Event ("c2", "ConferenceRegistered", "conf2"));
			es.Record (new Event ("c2s1", "SessionRegistered", "sess21\t2015-02-08T09:15:00\t2015-02-08T10:15:00\tname3\tname3@gmail.com"));
			es.Record (new Event("c2", "SessionAssigned", "c2s1"));
			es.Record (new Event ("c2s2", "SessionRegistered", "sess22\t2015-02-08T10:15:00\t2015-02-08T11:15:00\tname3\tname3@gmail.com"));
			es.Record (new Event("c2", "SessionAssigned", "c2s2"));
			es.Record (new Event ("c2s3", "SessionRegistered", "sess23\t2015-02-08T11:15:00\t2015-02-08T12:15:00\tname4\tname4@gmail.com"));
			es.Record (new Event("c2", "SessionAssigned", "c2s2"));
			var nEventsBefore = es.Replay ().Count ();

			TimeProvider.Configure (new DateTime (2015, 2, 8, 11, 0, 0));

			// act
			fakeNotifier.Clear ();
			body.Start_background_speaker_notification (20, 5);

			// assert
			var notificationsSent = fakeNotifier.Notifications;
			Assert.AreEqual (nEventsBefore + notificationsSent.Count, es.Replay ().Count());
			Assert.AreEqual (2, notificationsSent.Count);
			Assert.AreEqual ("sess11", notificationsSent [0].Title);
			Assert.AreEqual ("sess21", notificationsSent [1].Title);
			//TODO: some more assertions needed ;-)

			// act again
			fakeNotifier.Clear ();
			body.Start_background_speaker_notification (20, 5);

			// assert
			notificationsSent = fakeNotifier.Notifications;
			Assert.AreEqual (0, notificationsSent.Count);
		}
	}


	class FakeNotificationProvider : INotificationProvider {
		public List<SpeakerNotificationData> Notifications;


		#region INotificationProvider implementation

		public void Send_feedback (SpeakerNotificationData data)
		{
			this.Notifications.Add (data);
		}

		#endregion


		public void Clear() {
			this.Notifications = new List<SpeakerNotificationData> ();
		}
	}


	class FakeSchedulingProvider : ISchedulingProvider {
		#region ISchedulingProvider implementation

		public void Start (int schedulerRepeatInterval, Action action)
		{
			action ();
		}

		public void Stop ()
		{

		}

		#endregion
	}


//		[Test]
//		public void Start_speaker_notification_scheduler()
//		{
//			// arange
//			const string conferenceTitle = "Conference 2015";
//			const int schedulerRepeatInterval = 10;
//			const int feedbackPeriod = 20;
//			var now = new DateTime(2015, 1, 4, 12, 0, 0);
//			var emailServiceMock = new Mock<IEmailService>();
//			var dataProvider = new Mock<INotificationDataProvider>();
//			var allSessions = GetAllSession(now, schedulerRepeatInterval, feedbackPeriod).ToList();
//			var contextMock = new Mock<IJobExecutionContext>();
//			var sut = new SpeakerNotificationJob(emailServiceMock.Object, dataProvider.Object, new Mapper(),
//				feedbackPeriod, schedulerRepeatInterval);
//
//			TimeProvider.Configure(now);
//			dataProvider.Setup(x => x.Get_all_sessions()).Returns(allSessions);
//			dataProvider.Setup(x => x.Get_conference_title(It.IsAny<string>())).Returns(conferenceTitle);
//			dataProvider.Setup(x => x.Get_scores(allSessions[1].Id)).Returns(GetScore1());
//			dataProvider.Setup(x => x.Get_scores(allSessions[2].Id)).Returns(GetScore2());
//
//			// act
//			sut.Execute(contextMock.Object);
//
//			// assert
//			emailServiceMock.Verify(x => x.Send_speaker_notification(It.IsAny<SpeakerNotificationData>()), Times.Exactly(2));
//			emailServiceMock.Verify(x => x.Send_speaker_notification(It.Is<SpeakerNotificationData>(data =>
//				data.ConferenceTitle == conferenceTitle && data.Session == allSessions[1] && 
//				data.Reds == 1 && data.Yellows == 1 && data.Greens == 4)));
//			emailServiceMock.Verify(x => x.Send_speaker_notification(It.Is<SpeakerNotificationData>(data =>
//				data.ConferenceTitle == conferenceTitle && data.Session == allSessions[2] &&
//				data.Reds == 2 && data.Yellows == 2 && data.Greens == 1)));
//		}
//
//		private static IEnumerable<ConferenceData.SessionData> GetAllSession(DateTime now, int workerInvocationInterval,
//			int feedbackPeriod)
//		{
//			return new List<ConferenceData.SessionData>
//			{
//				new ConferenceData.SessionData // In the past.
//				{
//					Id = "1",
//					Start = now.AddDays(-1).AddHours(-1),
//					End = now.AddDays(-1)
//				},
//				new ConferenceData.SessionData // Due. Feedback period just finished a minute ago.
//				{
//					Id = "2",
//					Start = now.AddMinutes(-(60 + feedbackPeriod)),
//					End = now.AddMinutes(-(feedbackPeriod + 1))
//				},
//				new ConferenceData.SessionData // Due. Feedback period finished and we are in the middle of worker invocation interval.
//				{
//					Id = "3",
//					Start = now.AddMinutes(-(60 + feedbackPeriod)),
//					End = now.AddMinutes(-(feedbackPeriod + workerInvocationInterval /2))
//				},
//				new ConferenceData.SessionData // Already processed. After feedback period and worker invocation interval.
//				{
//					Id = "4",
//					Start = now.AddHours(-(1 + feedbackPeriod)),
//					End = now.AddMinutes(-(feedbackPeriod + workerInvocationInterval))
//				},
//				new ConferenceData.SessionData // Worker invoked at the same time as feedback period finishes -> Not due yet.
//				{
//					Id = "5",
//					Start = now.AddMinutes(-(60 + feedbackPeriod)),
//					End = now.AddMinutes(-(feedbackPeriod))
//				},
//			};
//		}
//
//		private static IEnumerable<TrafficLightScores> GetScore1()
//		{
//			return new[]
//			{
//				TrafficLightScores.Red, TrafficLightScores.Green, TrafficLightScores.Yellow,
//				TrafficLightScores.Green, TrafficLightScores.Green, TrafficLightScores.Green
//			};
//		}
//
//		private static IEnumerable<TrafficLightScores> GetScore2()
//		{
//			return new[]
//			{
//				TrafficLightScores.Red, TrafficLightScores.Yellow, TrafficLightScores.Red,
//				TrafficLightScores.Green, TrafficLightScores.Yellow
//			};
//		}
//
//	}
}

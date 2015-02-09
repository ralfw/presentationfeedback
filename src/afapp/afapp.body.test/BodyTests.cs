using afapp.body.contract;
using afapp.body.data;
using afapp.body.domain;
using afapp.body.providers;
using EventStore.Internals;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;


namespace afapp.body.test
{
	[TestFixture]
	class BodyTests
	{
		[Test]
		public void Speaker_notification_calculates_correct_due_sessions() {
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
			Assert.AreEqual("name1@gmail.com", notificationsSent[0].SpeakerEmail);
			Assert.AreEqual("name1", notificationsSent[0].SpeakerName);
			Assert.AreEqual(new DateTime(2015, 2, 8, 9, 0, 0), notificationsSent[0].Start);
			Assert.AreEqual(new DateTime(2015, 2, 8, 10, 0, 0), notificationsSent[0].End);

			Assert.AreEqual ("sess21", notificationsSent [1].Title);
			Assert.AreEqual("name3@gmail.com", notificationsSent[1].SpeakerEmail);
			Assert.AreEqual("name3", notificationsSent[1].SpeakerName);
			Assert.AreEqual(new DateTime(2015, 2, 8, 9, 15, 0), notificationsSent[1].Start);
			Assert.AreEqual(new DateTime(2015, 2, 8, 10, 15, 0), notificationsSent[1].End);

			// act again
			fakeNotifier.Clear ();
			body.Start_background_speaker_notification (20, 5);

			// assert
			notificationsSent = fakeNotifier.Notifications;
			Assert.AreEqual (0, notificationsSent.Count);
		}

		[Test]
		public void Speaker_notification_calculates_correct_feedback()
		{
			// arrange
			var es = new EventStore.InMemoryEventStore();
			var repo = new Repository(es);
			var map = new Mapper();
			var fakeScheduler = new FakeSchedulingProvider();
			var fakeNotifier = new FakeNotificationProvider();
			var scoredSessionsFactory = new Func<IEnumerable<ScoredSessionData>, ScoredSessions>((data) => new ScoredSessions(data));
			var body = new Body(repo, null, map, fakeScheduler, fakeNotifier, scoredSessionsFactory);

			es.Record(new Event("c1", "ConferenceRegistered", "conf1"));
			es.Record(new Event("c1s1", "SessionRegistered", "sess11\t2015-02-08T09:00:00\t2015-02-08T10:00:00\tname1\tname1@gmail.com"));
			es.Record(new Event("c1", "SessionAssigned", "c1s1"));
			es.Record(new Event("c1s2", "SessionRegistered", "sess12\t2015-02-08T10:00:00\t2015-02-08T11:00:00\tname2\tname2@gmail.com"));
			es.Record(new Event("c1", "SessionAssigned", "c1s2"));

			es.Record(new Event("c1s1", "FeedbackGiven", "Green\tGreat session\tjohn@doe.com"));
			es.Record(new Event("c1s1", "FeedbackGiven", "Red\tBoring!\tjane@doe.com"));
			es.Record(new Event("c1s1", "FeedbackGiven", "Yellow\t\tpeter@mail.com"));
			es.Record(new Event("c1s1", "FeedbackGiven", "Green\t\tpeter@mail.com")); // Attendee changed his mind. Last score is used.
			es.Record(new Event("c1s1", "FeedbackGiven", "Yellow\t\tmartin@mail.com"));

			es.Record(new Event("c1s2", "FeedbackGiven", "Green\t\tpeter@mail.com"));
			es.Record(new Event("c1s2", "FeedbackGiven", "Yellow\t\tpeter@mail.com"));

			TimeProvider.Configure(new DateTime(2015, 2, 8, 11, 0, 0));
			fakeNotifier.Clear();

			// act
			body.Start_background_speaker_notification(20, 5);

			// assert
			var notificationsSent = fakeNotifier.Notifications;
			Assert.AreEqual(1, notificationsSent.Count);
			Assert.AreEqual(2, notificationsSent[0].Greens);
			Assert.AreEqual(1, notificationsSent[0].Reds);
			Assert.AreEqual(1, notificationsSent[0].Yellows);
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
}

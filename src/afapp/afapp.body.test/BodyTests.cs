using afapp.body.domain;
using afapp.body.providers;
using Contract;
using Contract.data;
using EventStore;
using EventStore.Contract;
using MongoDB.Driver;
using NUnit.Framework;
using Repository.data;
using Repository.events;
using System;
using System.Collections.Generic;
using System.Linq;


namespace afapp.body.test
{

	[TestFixture]
	class BodyTests
	{
		private const string ConnectionString = "mongodb://admin:admin@dogen.mongohq.com:10097/trafficlightfeedback_test";
		private const string Database = "trafficlightfeedback_test";

		[SetUp]
		public void Init()
		{
			var client = new MongoClient(ConnectionString);
			var server = client.GetServer();
			var database = server.GetDatabase(Database);
			database.GetCollection<IRecordedEvent>("events").Drop();
		}

		[Test]
		public void Speaker_notification_calculates_correct_due_sessions() {
			// arrange
			var es = new MongoEventStore(ConnectionString, Database);
			var repo = new Repository.Repository (es);
			var map = new Mapper ();
			var fakeScheduler = new FakeSchedulingProvider ();
			var fakeNotifier = new FakeNotificationProvider ();
			var scoredSessionsFactory = new Func<IEnumerable<ScoredSessionData>, ScoredSessions> (data => new ScoredSessions(data));

			var body = new Body (repo, null, map, fakeScheduler, fakeNotifier, scoredSessionsFactory);

			// It would have been nice to have the coapp Repository{} available.
			es.Record (new ConferenceRegistered("c1", "conf1"));
			es.Record(new SessionRegistered("c1s1", "sess11", new DateTime(2015, 02, 08, 09, 00, 00, DateTimeKind.Utc), 
				new DateTime(2015, 02, 08, 10, 00, 00, DateTimeKind.Utc), "name1", "name1@gmail.com"));
			es.Record (new SessionAssigned("c1", "c1s1"));
			es.Record(new SessionRegistered("c1s2", "sess12", new DateTime(2015, 02, 08, 10, 00, 00, DateTimeKind.Utc), 
				new DateTime(2015, 02, 08, 11, 00, 00, DateTimeKind.Utc), "name2", "name2@gmail.com"));
			es.Record (new SessionAssigned("c1", "c1s2"));
			es.Record(new SessionRegistered("c1s3", "sess13", new DateTime(2015, 02, 08, 11, 00, 00, DateTimeKind.Utc), 
				new DateTime(2015, 02, 08, 12, 00, 00, DateTimeKind.Utc), "name1", "name1@gmail.com"));
			es.Record (new SessionAssigned("c1", "c1s3"));

			es.Record (new ConferenceRegistered("c2", "conf2"));
			es.Record(new SessionRegistered("c2s1", "sess21", new DateTime(2015, 02, 08, 09, 15, 00, DateTimeKind.Utc),
				new DateTime(2015, 02, 08, 10, 15, 00, DateTimeKind.Utc), "name3", "name3@gmail.com"));
			es.Record (new SessionAssigned("c2", "c2s1"));
			es.Record(new SessionRegistered("c2s2", "sess22", new DateTime(2015, 02, 08, 10, 15, 00, DateTimeKind.Utc), 
				new DateTime(2015, 02, 08, 11, 15, 00, DateTimeKind.Utc), "name3", "name3@gmail.com"));
			es.Record (new SessionAssigned("c2", "c2s2"));
			es.Record(new SessionRegistered("c2s3", "sess23", new DateTime(2015, 02, 08, 11, 15, 00, DateTimeKind.Utc), 
				new DateTime(2015, 02, 08, 12, 15, 00, DateTimeKind.Utc), "name4", "name4@gmail.com"));
			es.Record (new SessionAssigned("c2", "c2s2"));
			var nEventsBefore = es.Replay ().Count ();

			TimeProvider.Configure(new DateTime(2015, 2, 8, 11, 0, 0, DateTimeKind.Utc));

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
			var es = new MongoEventStore(ConnectionString, Database);
			var repo = new Repository.Repository(es);
			var map = new Mapper();
			var fakeScheduler = new FakeSchedulingProvider();
			var fakeNotifier = new FakeNotificationProvider();
			var scoredSessionsFactory = new Func<IEnumerable<ScoredSessionData>, ScoredSessions>(data => new ScoredSessions(data));
			var body = new Body(repo, null, map, fakeScheduler, fakeNotifier, scoredSessionsFactory);

			es.Record(new ConferenceRegistered("c1", "conf1"));
			es.Record(new SessionRegistered("c1s1", "sess11", new DateTime(2015, 02, 08, 09, 00, 00, DateTimeKind.Utc), 
				new DateTime(2015, 02, 08, 10, 00, 00, DateTimeKind.Utc), "name1", "name1@gmail.com"));
			es.Record(new SessionAssigned("c1", "c1s1"));
			es.Record(new SessionRegistered("c1s2", "sess12", new DateTime(2015, 02, 08, 10, 00, 00, DateTimeKind.Utc), 
				new DateTime(2015, 02, 08, 11, 00, 00, DateTimeKind.Utc), "name2", "name2@gmail.com"));
			es.Record(new SessionAssigned("c1", "c1s2"));

			es.Record(new FeedbackGiven("c1s1", TrafficLightScores.Green, "Great session", "john@doe.com"));
			es.Record(new FeedbackGiven("c1s1", TrafficLightScores.Red, "Boring!", "jane@doe.com"));
			es.Record(new FeedbackGiven("c1s1", TrafficLightScores.Yellow, "", "peter@mail.com"));
			es.Record(new FeedbackGiven("c1s1", TrafficLightScores.Green, "", "peter@mail.com")); // Attendee changed his mind. Last score is used.
			es.Record(new FeedbackGiven("c1s1", TrafficLightScores.Yellow, "", "martin@mail.com"));

			es.Record(new FeedbackGiven("c1s2", TrafficLightScores.Green, "", "peter@mail.com"));
			es.Record(new FeedbackGiven("c1s2", TrafficLightScores.Yellow, "", "peter@mail.com"));

			TimeProvider.Configure(new DateTime(2015, 2, 8, 11, 0, 0, DateTimeKind.Utc));
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

		[Test]
		public void Generate_conference_overview()
		{
			// arrange
			var es = new MongoEventStore(ConnectionString, Database);
			var repo = new Repository.Repository(es);
			var map = new Mapper();
			var fakeScheduler = new FakeSchedulingProvider();
			var fakeNotifier = new FakeNotificationProvider();
			var scoredSessionsFactory = new Func<IEnumerable<ScoredSessionData>, ScoredSessions>(data => new ScoredSessions(data));
			var conferenceFactory = new Func<ConferenceData, Conference>(data => new Conference(data));
			var su = new Body(repo, conferenceFactory, map, fakeScheduler, fakeNotifier, scoredSessionsFactory);

			es.Record(new ConferenceRegistered("c1", "conf1"));
			es.Record(new SessionRegistered("c1s1", "sess11", new DateTime(2015, 02, 08, 09, 00, 00, DateTimeKind.Utc), 
				new DateTime(2015, 02, 08, 10, 00, 00, DateTimeKind.Utc), "name1", "name1@gmail.com"));
			es.Record(new SessionAssigned("c1", "c1s1"));
			es.Record(new SessionRegistered("c1s2", "sess12", new DateTime(2015, 02, 09, 10, 00, 00, DateTimeKind.Utc), 
				new DateTime(2015, 02, 09, 11, 00, 00, DateTimeKind.Utc), "name2", "name2@gmail.com"));
			es.Record(new SessionAssigned("c1", "c1s2"));
			es.Record(new SessionRegistered("c1s3", "sess13", new DateTime(2015, 02, 10, 11, 00, 00, DateTimeKind.Utc), 
				new DateTime(2015, 02, 10, 12, 00, 00, DateTimeKind.Utc), "name1", "name1@gmail.com"));
			es.Record(new SessionAssigned("c1", "c1s3"));

			es.Record(new ConferenceRegistered("c2", "conf2"));
			es.Record(new SessionRegistered("c2s2", "sess22", new DateTime(2015, 03, 05, 10, 15, 00, DateTimeKind.Utc), 
				new DateTime(2015, 03, 05, 11, 15, 00, DateTimeKind.Utc), "name3", "name3@gmail.com"));
			es.Record(new SessionAssigned("c2", "c2s2"));
			es.Record(new SessionRegistered("c2s1", "sess21", new DateTime(2015, 03, 05, 09, 15, 00, DateTimeKind.Utc), 
				new DateTime(2015, 03, 05, 10, 15, 00, DateTimeKind.Utc), "name3", "name3@gmail.com"));
			es.Record(new SessionAssigned("c2", "c2s1"));
			es.Record(new SessionRegistered("c2s3", "sess23", new DateTime(2015, 03, 05, 11, 15, 00, DateTimeKind.Utc), 
			new DateTime(2015, 03, 05, 12, 15, 00, DateTimeKind.Utc), "name4", "name4@gmail.com"));
			es.Record(new SessionAssigned("c2", "c2s3"));

			// act
			var result = su.Generate_conference_overview().ToList();

			// assert
			Assert.AreEqual(2, result.Count());

			Assert.AreEqual("c1", result[0].Id);
			Assert.AreEqual("conf1", result[0].Title);
			Assert.AreEqual(new DateTime(2015, 02, 08, 09, 00, 00, DateTimeKind.Utc), result[0].Start);
			Assert.AreEqual(new DateTime(2015, 02, 10, 12, 00, 00, DateTimeKind.Utc), result[0].End);

			Assert.AreEqual("c2", result[1].Id);
			Assert.AreEqual("conf2", result[1].Title);
			Assert.AreEqual(new DateTime(2015, 03, 05, 09, 15, 00, DateTimeKind.Utc), result[1].Start);
			Assert.AreEqual(new DateTime(2015, 03, 05, 12, 15, 00, DateTimeKind.Utc), result[1].End);
		}
	}

	class FakeNotificationProvider : INotificationProvider {
		public List<SpeakerNotificationData> Notifications;


		#region INotificationProvider implementation

		public void Send_feedback (SpeakerNotificationData data)
		{
			Notifications.Add (data);
		}

		#endregion


		public void Clear() {
			Notifications = new List<SpeakerNotificationData> ();
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

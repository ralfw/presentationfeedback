using afapp.body.domain;
using EventStore;
using EventStore.Contract;
using MongoDB.Driver;
using NUnit.Framework;
using Repository.data;
using Repository.events;
using System;
using System.Linq;

namespace afapp.body.test
{
	using MongoDB.Bson.Serialization;
	using MongoDB.Bson.Serialization.Options;
	using MongoDB.Bson.Serialization.Serializers;

	[TestFixture]
	class BodyTests
	{
		private const string ConnectionString = "mongodb://admin:admin@dogen.mongohq.com:10097/trafficlightfeedback_test";
		private const string Database = "trafficlightfeedback_test";

		static BodyTests()
		{
			BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeSerializationOptions.LocalInstance));
		}

		[SetUp]
		public void Init()
		{
			var client = new MongoClient(ConnectionString);
			var server = client.GetServer();
			var database = server.GetDatabase(Database);
			database.GetCollection<IRecordedEvent>("events").Drop();
		}

		[Test]
		public void Generate_conference_overview()
		{
			// arrange 
			const string timeZone = "FLE Standard Time"; 
			var es = new MongoEventStore(ConnectionString, Database);
			var repo = new Repository.Repository(es);
			var map = new Mapper();
			var conferenceFactory = new Func<ConferenceData, Conference>(data => new Conference(data, 20));
			var su = new Body(repo, conferenceFactory, map);

			es.Record(new ConferenceRegistered("c1", "conf1", timeZone));
			es.Record(new SessionRegistered("c1s1", "sess11", new DateTime(2015, 02, 08, 09, 00, 00),
				new DateTime(2015, 02, 08, 10, 00, 00), timeZone, "name1", "name1@gmail.com"));
			es.Record(new SessionAssigned("c1", "c1s1"));
			es.Record(new SessionRegistered("c1s2", "sess12", new DateTime(2015, 02, 09, 10, 00, 00),
				new DateTime(2015, 02, 09, 11, 00, 00), timeZone, "name2", "name2@gmail.com"));
			es.Record(new SessionAssigned("c1", "c1s2"));
			es.Record(new SessionRegistered("c1s3", "sess13", new DateTime(2015, 02, 10, 11, 00, 00),
				new DateTime(2015, 02, 10, 12, 00, 00), timeZone, "name1", "name1@gmail.com"));
			es.Record(new SessionAssigned("c1", "c1s3"));

			es.Record(new ConferenceRegistered("c2", "conf2", timeZone));
			es.Record(new SessionRegistered("c2s2", "sess22", new DateTime(2015, 03, 05, 10, 15, 00),
				new DateTime(2015, 03, 05, 11, 15, 00), timeZone, "name3", "name3@gmail.com"));
			es.Record(new SessionAssigned("c2", "c2s2"));
			es.Record(new SessionRegistered("c2s1", "sess21", new DateTime(2015, 03, 05, 09, 15, 00),
				new DateTime(2015, 03, 05, 10, 15, 00), timeZone, "name3", "name3@gmail.com"));
			es.Record(new SessionAssigned("c2", "c2s1"));
			es.Record(new SessionRegistered("c2s3", "sess23", new DateTime(2015, 03, 05, 11, 15, 00),
				new DateTime(2015, 03, 05, 12, 15, 00), timeZone, "name4", "name4@gmail.com"));
			es.Record(new SessionAssigned("c2", "c2s3"));

			// act
			var result = su.Generate_conference_overview().ToList();

			// assert
			var expectedStart = new DateTime(2015, 02, 08, 09, 00, 00);
			var expectedEnd = new DateTime(2015, 02, 10, 12, 00, 00);
			var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
			Assert.AreEqual(2, result.Count());
			Assert.AreEqual("c1", result[0].Id);
			Assert.AreEqual("conf1", result[0].Title);
			Assert.AreEqual(expectedStart, result[0].Start.LocalTime);
			Assert.AreEqual(TimeZoneInfo.ConvertTimeToUtc(expectedStart, zone), result[0].Start.UtcTime);
			Assert.AreEqual(expectedEnd, result[0].End.LocalTime);
			Assert.AreEqual(TimeZoneInfo.ConvertTimeToUtc(expectedEnd, zone), result[0].End.UtcTime);

			Assert.AreEqual("c2", result[1].Id);
			Assert.AreEqual("conf2", result[1].Title);
			Assert.AreEqual(new DateTime(2015, 03, 05, 09, 15, 00), result[1].Start.LocalTime);
			Assert.AreEqual(new DateTime(2015, 03, 05, 12, 15, 00), result[1].End.LocalTime);
		}
	}

}

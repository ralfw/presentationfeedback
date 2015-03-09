using afapp.body;
using afapp.body.domain;
using afapp.body.providers;
using EventStore;
using Repository.data;
using System;

namespace afapp.console
{
	using System.Collections.Generic;

	class MainClass
	{
		public static void Main (string[] args)
		{
			TimeProvider.Configure ();
			const string connectionString = "mongodb://admin:admin@dogen.mongohq.com:10046/trafficlightfeedback";
			const string database = "trafficlightfeedback";
			var es = new MongoEventStore(connectionString, database);
			var repo = new Repository.Repository (es);
			var conferenceFactory = new Func<ConferenceData, Conference> (data => new Conference(data));
			var scoredSessions = new Func<IEnumerable<ScoredSessionData>, ScoredSessions>(data => new ScoredSessions(data));
			var mapper = new Mapper ();
			var scheduler = new SchedulingProvider ();
			var emailNotifier = new FakeEmailNotificationProvider();
			var body = new Body (repo, conferenceFactory, mapper, scheduler, emailNotifier, scoredSessions);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

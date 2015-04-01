using afapp.body;
using afapp.body.domain;
using Contract.provider;
using EventStore;
using Repository.data;
using System;

namespace afapp.console
{

	class MainClass
	{
		public static void Main (string[] args)
		{
			TimeProvider.Configure ();
			const string connectionString = "mongodb://admin:admin@dogen.mongohq.com:10046/trafficlightfeedback";
			const string database = "trafficlightfeedback";
			var es = new MongoEventStore(connectionString, database);
			var repo = new Repository.Repository (es);
			var conferenceFactory = new Func<ConferenceData, Conference> (data => new Conference(data, 20));
			var mapper = new Mapper ();
			var body = new Body (repo, conferenceFactory, mapper);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

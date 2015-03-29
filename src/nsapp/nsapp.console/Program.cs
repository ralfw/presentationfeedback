using Contract.provider;
using EventStore;
using nsapp.body;
using nsapp.body.domain;
using nsapp.body.providers;
using Repository.data;
using System;
using System.Collections.Generic;

namespace nsapp.console
{
	class Program
	{
		static void Main(string[] args)
		{
			TimeProvider.Configure();
			const string connectionString = "mongodb://admin:admin@dogen.mongohq.com:10046/trafficlightfeedback";
			const string database = "trafficlightfeedback";
			var es = new MongoEventStore(connectionString, database);
			var repo = new Repository.Repository(es);
			var scoredSessions = new Func<IEnumerable<ScoredSessionData>, ScoredSessions>(data => new ScoredSessions(data));
			var mapper = new Mapper();
			var scheduler = new SchedulingProvider();
			var emailNotifier = new FakeEmailNotificationProvider();
			var body = new Body(repo, mapper, scheduler, emailNotifier, scoredSessions);
			var head = new Head(body);

			head.Run(args);

		}
	}
}

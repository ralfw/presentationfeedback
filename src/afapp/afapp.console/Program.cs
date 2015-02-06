using afapp.body;
using afapp.body.data;
using afapp.body.domain;
using afapp.body.providers;
using EventStore;
using System;

namespace afapp.console
{
	using System.Collections.Generic;

	class MainClass
	{
		public static void Main (string[] args)
		{
			TimeProvider.Configure ();

			var es = new FileEventStore ("app.events");
			var repo = new Repository (es);
			var conferenceFactory = new Func<ConferenceData, Conference> (data => new Conference(data));
			var sessionsFactory = new Func<IEnumerable<SessionWithScoresData>, Sessions>(data => new Sessions(data));
			var mapper = new Mapper ();
			var scheduler = new SchedulingProvider ();
			var emailService = new FakeEmailService();
			var body = new Body (repo, conferenceFactory, mapper, scheduler, emailService, sessionsFactory);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

using afapp.body;
using afapp.body.data;
using afapp.body.domain;
using EventStore;
using System;
using afapp.body.providers;

namespace afapp.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			TimeProvider.Configure ();

			var es = new FileEventStore ("app.events");
			var repo = new Repository (es);
			var conferenceFactory = new Func<ConferenceData, Conference> ((data) => new Conference(data));
			var mapper = new Mapper ();
			var scheduler = new SchedulingProvider ();
			var emailService = new FakeEmailService();
			var body = new Body (repo, conferenceFactory, mapper, scheduler, emailService);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

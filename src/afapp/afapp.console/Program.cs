using afapp.body;
using EventStore;
using System;
using System.Linq;
using afapp.body.data;

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
			var body = new Body (repo, conferenceFactory, mapper);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

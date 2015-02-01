using afapp.body;
using EventStore;
using System;
using System.Linq;

namespace afapp.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var es = new FileEventStore ("app.events");
			var repo = new Repository (es);
			var conferenceFactory = new Func<ConferenceData, Func<DateTime>, Conference> ((data, now) => new Conference(data, now));
			var mapper = new Mapper ();
			var body = new Body (repo, conferenceFactory, mapper);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

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
			var body = new Body (repo, conferenceFactory, mapper, BuildCurrentTimeProvider(args));
			var head = new Head (body);

			head.Run (args);
		}
			
		private static Func<DateTime> BuildCurrentTimeProvider(string[] args) {
			const string NOW_PREFIX = "-now:";
			var nowArg = args.FirstOrDefault (a => a.StartsWith(NOW_PREFIX));
			if (nowArg == null)	return () => DateTime.Now;

			var currentTime = DateTime.Parse (nowArg.Substring (NOW_PREFIX.Length));
			return () => currentTime;
		}
	}
}

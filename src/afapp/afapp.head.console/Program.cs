using System;
using afapp.body;
using EventStore;
using System.Collections.Generic;

namespace afapp.head.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var es = new FileEventStore ("app.events");
			var repo = new Repository (es);
			var body = new Body (repo, () => DateTime.Now);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

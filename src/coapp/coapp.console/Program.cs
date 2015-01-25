using System;
using coapp.body;

namespace coapp.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var es = new EventStore.FileEventStore ("coapp.events");
			var repo = new Repository (es);
			var parser = new CSVParser ();

			var body = new Body (parser, repo);
			var head = new Head (body);

			head.Run (args);
		}
	}


}

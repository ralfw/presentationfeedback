using coapp.body;

namespace coapp.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var es = new EventStore.FileEventStore ("app.events");
			var repo = new Repository.Repository(es);
			var body = new Body (repo);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

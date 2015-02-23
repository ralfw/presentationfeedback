using coapp.body;

namespace coapp.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var es = new EventStore.FileEventStore ("app.events");
			var body = new Body (es);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

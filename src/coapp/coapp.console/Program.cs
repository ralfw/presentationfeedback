using coapp.body;

namespace coapp.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			const string connectionString = "mongodb://admin:admin@dogen.mongohq.com:10046/trafficlightfeedback";
			const string database = "trafficlightfeedback";
			var es = new EventStore.MongoEventStore(connectionString, database);
			var body = new Body (es);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

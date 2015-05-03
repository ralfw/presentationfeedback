using coapp.body;

namespace coapp.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			const string connectionString = "mongodb://admin:admin@ds063769.mongolab.com:63769/presentationfeedback"; 
			const string database = "presentationfeedback";
			var es = new EventStore.MongoEventStore(connectionString, database);
			var body = new Body (es);
			var head = new Head (body);

			head.Run (args);
		}
	}
}

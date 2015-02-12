using EventStore.Contract;
using Repository.data;

namespace coapp.body
{
	public class Body
	{
		readonly CSVParser parser;
		readonly Repository.Repository repo;

		public Body (IEventStore es)	{
			this.repo = new Repository.Repository(es);
			this.parser = new CSVParser ();
		}

		public int RegisterConference(string id, string title, string csvSessions) {
			this.repo.StoreConference (id, title);
			var sessions = this.parser.ParseSessions (csvSessions);
			return this.repo.StoreSessions (id, sessions);
		}
	}
}


using System;
using System.Collections.Generic;
using EventStore.Contract;

namespace coapp.body
{
	public class Body
	{
		CSVParser parser;
		Repository repo;

		public Body (IEventStore es)	{
			this.repo = new Repository (es);
			this.parser = new CSVParser ();
		}


		public int RegisterConference(string id, string title, string csvSessions) {
			this.repo.StoreConference (id, title);
			var sessions = this.parser.ParseSessions (csvSessions);
			return this.repo.StoreSessions (id, sessions);
		}
	}
}


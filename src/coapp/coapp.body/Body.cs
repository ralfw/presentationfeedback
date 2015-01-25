using System;
using System.Collections.Generic;
using EventStore.Contract;

namespace coapp.body
{
	public class Body
	{
		CSVParser parser;
		Repository repo;

		public Body (CSVParser parser, Repository repo)	{
			this.repo = repo;
			this.parser = parser;
		}


		public int RegisterConference(string id, string title, string csvSessions) {
			this.repo.StoreConference (id, title);
			var sessions = this.parser.ParseSessions (csvSessions);
			return this.repo.StoreSessions (id, sessions);
		}
	}
}


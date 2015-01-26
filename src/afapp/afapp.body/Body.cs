using System;
using EventStore.Contract;

namespace afapp.body
{
	public class Body
	{
		Repository repo;
		Conference conf;
		Mapper map;

		public Body (Repository repo, Conference conf, Mapper map) {
			this.repo = repo;
			this.conf = conf;
			this.map = map;
		}


		public SessionOverviewVM GenerateSessionOverview(string confId) {

		}
	}

	struct SessionOverviewVM {

	}


	public class Conference {

	}


	public class Repository {
		public Repository(IEventStore es) {

		}
	}

	public class ConferenceData {

	}


	public class Mapper {

	}
}


using System;
using EventStore.Contract;
using System.Collections.Generic;

namespace afapp.body
{

	public class Repository {
		IEventStore es;

		public Repository(IEventStore es) {
			this.es = es;
		}
			
		public ConferenceData LoadConference(string confId) {
			throw new NotImplementedException ();
		}
	}

}

using System;
using EventStore.Contract;
using System.Collections.Generic;

namespace afapp.body
{

	public class Mapper {
		public static SessionOverviewVM Map(string confId, string confTitle, IEnumerable<SessionData> activeSessions, IEnumerable<SessionData> inactiveSessions) {
			throw new NotImplementedException ();
		}
	}
}

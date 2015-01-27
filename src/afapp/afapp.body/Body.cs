using System;
using EventStore.Contract;
using System.Collections.Generic;

namespace afapp.body
{
	public class Body
	{
		private Repository repo;
		private Func<DateTime> now;

		public Body (Repository repo, Func<DateTime> now) {
			this.repo = repo;
			this.now = now;
		}
			
		public SessionOverviewVM GenerateSessionOverview(string confId) {
			var confdata = this.repo.LoadConference (confId);

			var conf = new Conference (confdata, this.now);
			var activeSessions = conf.ActiveSessions;
			var inactiveSessions = conf.InactiveSessions;

			return Mapper.Map (confdata.Id, confdata.Title, activeSessions, inactiveSessions);
		}
	}
}


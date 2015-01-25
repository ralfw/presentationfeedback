using System;
using System.Collections.Generic;
using EventStore.Contract;
using System.Linq;
using EventStore.Internals;

namespace coapp.body
{
	public class Repository {
		IEventStore es;

		public Repository(IEventStore es) {
			this.es = es;
		}


		public void StoreConference(string id, string title) {
			var e = new Event (id, "ConferenceRegistered", title);
			this.es.Record (e);
		}


		public int StoreSessions(string conferenceId, IEnumerable<SessionParsed> sessions) {
			var n = 0;
			foreach (var s in sessions) {
				var e = new Event (s.Id, "SessionRegistered", 
								   string.Format ("{0}\t{1:s}\t{2:s}\t{3}\t{4}", 
												  s.Title, s.Start, s.End, s.SpeakerName, s.SpeakerEmail));
				this.es.Record (e);

				e = new Event (conferenceId, "SessionAssigned", s.Id);
				this.es.Record (e);

				n++;
			}
			return n;
		}
	}
}

using EventStore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using afapp.body.data;

namespace afapp.body
{
	using EventStore.Internals;

	public class Repository {
		readonly IEventStore es;

		public Repository(IEventStore es) {
			this.es = es;
		}
			

		public ConferenceData LoadConference(string confId) {
			var events = this.es.QueryByContext (confId);

			var confdata = new ConferenceData { Id = confId };
			var sessionIds = new HashSet<string> ();
			foreach (var e in events) {
				switch (e.Name) {
				case "ConferenceRegistered":
					confdata.Title = e.Payload;
					break;
				case "SessionAssigned":
					sessionIds.Add (e.Payload);
					break;
				}
			}

			events = this.es.QueryByName ("SessionRegistered")
							.Where (e => sessionIds.Contains (e.Context));
			var confSessions = new List<SessionData> ();
			foreach (var e in events) {
				var fields = e.Payload.Split ('\t');

				var sessiondata = new SessionData{ 
					Id = e.Context,
					Title = fields[0],
					Start = DateTime.Parse(fields[1]),
					End = DateTime.Parse(fields[2]),
					SpeakerName = fields[3],
					SpeakerEmail = fields[4]
				};
				confSessions.Add (sessiondata);
			}
			 
			confdata.Sessions = confSessions;
			return confdata;
		}


		public void Register_feedback(FeedbackData feedback)
		{
			es.Record(new Event(feedback.SessionId, "FeedbackGiven", 
								string.Format("{0}\t{1}\t{2}", feedback.Score, feedback.Comment, feedback.Email)));
		}
	}
}

using afapp.body.data;
using afapp.body.data.contract;
using afapp.body.speakerNotification;
using EventStore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body
{
	using EventStore.Internals;

	public class Repository : INotificationDataProvider {
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
			var confSessions = new List<ConferenceData.SessionData> ();
			foreach (var e in events) {
				var fields = e.Payload.Split ('\t');

				var sessiondata = new ConferenceData.SessionData{ 
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

		public IEnumerable<ConferenceData.SessionData> Get_all_sessions()
		{
			return es.QueryByName("SessionRegistered").Select(e =>
			{
				var fields = e.Payload.Split('\t');
				return new ConferenceData.SessionData
				{
					Id = e.Context,
					Title = fields[0],
					Start = DateTime.Parse(fields[1]),
					End = DateTime.Parse(fields[2]),
					SpeakerName = fields[3],
					SpeakerEmail = fields[4]
				};
			});
		}

		public string Get_conference_title(string sessionId)
		{
			var confId = es.QueryByName("SessionAssigned").First(x => x.Payload.Contains(sessionId)).Context;
			return es.QueryByName("ConferenceRegistered").Single(x => x.Context == confId).Payload;
		}

		public IEnumerable<TrafficLightScores> Get_scores(string sessionId)
		{
			return es.QueryByName("FeedbackGiven").Where(e => e.Context == sessionId).Select(s =>
			{
				var fields = s.Payload.Split('\t');
				return (TrafficLightScores)Enum.Parse(typeof(TrafficLightScores), fields[0]);
			});
		}
	}
}

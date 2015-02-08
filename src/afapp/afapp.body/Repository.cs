using afapp.body.contract.data;
using afapp.body.data;
using EventStore.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body
{
	using EventStore.Internals;

	public class Repository {
		readonly IEventStore es;

		public Repository(IEventStore es) {
			this.es = es;
		}


		public ConferenceData Load_conference(string confId) {
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


		public IEnumerable<ScoredSessionData> Load_scored_sessions()
		{
			var events = this.es.Replay ();

			var scoredSessions = new Dictionary<string, ScoredSessionData> ();
			var conferences = new Dictionary<string, string> ();
			foreach (var e in events) {
				switch (e.Name) {
				case "ConferenceRegistered":
					conferences.Add (e.Context, e.Payload);
					break;
				case "SessionRegistered": {
						var fields = e.Payload.Split ('\t');
						var scoredSession = new ScoredSessionData { 
							Id = e.Context,
							Title = fields [0],
							Start = DateTime.Parse (fields [1]),
							End = DateTime.Parse (fields [2]),
							SpeakerName = fields [3],
							SpeakerEmail = fields [4]
						};
						scoredSessions.Add (e.Context, scoredSession);
					}
					break;
				case "SessionAssigned": {
						var confTitle = conferences [e.Context];
						var scoredSession = scoredSessions [e.Payload];
						scoredSession.ConfTitle = confTitle;
					}
					break;
				case "SpeakerNotified": {
						var scoredSession = scoredSessions [e.Context];
						scoredSession.SpeakerNotified = true;
					}
					break;
				case "FeedbackGiven": {
						var fields = e.Payload.Split('\t');
						var feedback = new FeedbackData {
							SessionId = e.Context,
							Score = (TrafficLightScores)Enum.Parse (typeof(TrafficLightScores), fields [0]),
							Comment = fields [1],
							Email = fields [2]
						};
						var scoredSession = scoredSessions [e.Context];
						scoredSession.Feedback.Add (feedback);
					}
					break;
				}
			}
			return scoredSessions.Values;
		}
			

		public void Register_feedback_notification(string sessionId)
		{
			es.Record(new Event(sessionId, "SpeakerNotified", ""));
		}
	}
}

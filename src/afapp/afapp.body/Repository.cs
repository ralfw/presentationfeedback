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

		public IEnumerable<SessionWithScoresData> Load_sessions()
		{
			return Get_sessions(
				sessionId =>
				{
					var confId = es.QueryByName("SessionAssigned").First(x => x.Payload.Contains(sessionId)).Context;
					return es.QueryByName("ConferenceRegistered").Single(x => x.Context == confId).Payload;
				},
				sessionId => es.QueryByName("FeedbackGiven").Where(e => e.Context == sessionId).Select(s =>
				{
					var fields = s.Payload.Split('\t');
					return (TrafficLightScores) Enum.Parse(typeof (TrafficLightScores), fields[0]);
				}),
				sessionId => es.QueryByName("SpeakerEmailSent").Select(x => x.Context).Contains(sessionId)
			);
		}

		private IEnumerable<SessionWithScoresData> Get_sessions(
			Func<string, string> getConfTitle, 
			Func<string, IEnumerable<TrafficLightScores>> getScores,
			Func<string, bool> isSpeakerNotified)
		{
			return es.QueryByName("SessionRegistered").Select(e =>
			{
				var fields = e.Payload.Split('\t');
				return new SessionWithScoresData
				{
					Id = e.Context,
					ConferenceTitle = getConfTitle,
					Scores = getScores,
					IsSpeakerNotifiedAboutSessionfeedback = isSpeakerNotified,
					Title = fields[0],
					Start = DateTime.Parse(fields[1]),
					End = DateTime.Parse(fields[2]),
					SpeakerName = fields[3],
					SpeakerEmail = fields[4],
				};
			});
		}

		public void Remember_speaker_got_notified_about_session_feedback(SessionWithScoresData session)
		{
			es.Record(new Event(session.Id, "SpeakerEmailSent", string.Format("{0}\t", session.SpeakerEmail)));
		}
	}
}

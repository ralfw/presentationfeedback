using EventStore.Contract;
using Repository.data;
using Repository.events;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
	public class Repository
	{
		readonly IEventStore es;

		public Repository(IEventStore es)
		{
			this.es = es;
		}

		public void StoreConference(string id, string title)
		{
			var e = new ConferenceRegistered(id, title);
			this.es.Record(e);
		}


		public int StoreSessions(string conferenceId, IEnumerable<SessionParsed> sessions)
		{
			var n = 0;
			foreach (var s in sessions)
			{
				this.es.Record(new SessionRegistered(s.Id, s.Title, s.Start, s.End, s.SpeakerName, s.SpeakerEmail));
				this.es.Record(new SessionAssigned(conferenceId, s.Id));
				n++;
			}
			return n;
		}

		public ConferenceData Load_conference(string confId)
		{
			var recordedEvents = this.es.QueryByContext(confId);

			var confdata = new ConferenceData { Id = confId };
			var sessionIds = new HashSet<string>();
			foreach (var e in recordedEvents)
			{
				switch (e.Event.Name)
				{
					case "ConferenceRegistered":
						confdata.Title = e.Event.Payload;
						break;
					case "SessionAssigned":
						sessionIds.Add(e.Event.Payload);
						break;
				}
			}

			recordedEvents = this.es.QueryByType(typeof(SessionRegistered)).Where(e => sessionIds.Contains(e.Event.Context));
			var confSessions = new List<ConferenceData.SessionData>();

			foreach (var e in recordedEvents)
			{
				var @event = (SessionRegistered) e.Event;
				var sessiondata = new ConferenceData.SessionData
				{
					Id = @event.SessionId,
					Title = @event.Title,
					Start = @event.Start,
					End = @event.End,
					SpeakerName = @event.SpeakerName,
					SpeakerEmail = @event.SpeakerEmail
				};
				confSessions.Add(sessiondata);
			}

			confdata.Sessions = confSessions;
			return confdata;
		}

		public void Register_feedback(FeedbackData feedback)
		{
			es.Record(new FeedbackGiven(feedback.SessionId, feedback.Score, feedback.Comment, feedback.Email));
		}


		public IEnumerable<ScoredSessionData> Load_scored_sessions()
		{
			var recordedEvents = this.es.Replay();
			var scoredSessions = new Dictionary<string, ScoredSessionData>();
			var conferences = new Dictionary<string, string>();
			foreach (var e in recordedEvents)
			{
				var @event = (ISemaphoreFeedbackEvent) e.Event;
				@event.Accept(
					conferenceRegistered => conferences.Add(conferenceRegistered.ConfId, conferenceRegistered.Title),
					sessionRegistered =>
					{
						var scoredSession = new ScoredSessionData
						{
							Id = sessionRegistered.SessionId,
							Title = sessionRegistered.Title,
							Start = sessionRegistered.Start,
							End = sessionRegistered.End,
							SpeakerName = sessionRegistered.SpeakerName,
							SpeakerEmail = sessionRegistered.SpeakerEmail
						};
						scoredSessions.Add(sessionRegistered.SessionId, scoredSession);
					},
					sessionAssigned =>
					{
						var confTitle = conferences[sessionAssigned.ConfId];
						var scoredSession = scoredSessions[sessionAssigned.SessionId];
						scoredSession.ConfTitle = confTitle;
					},
					speakerNotified =>
					{
						var scoredSession = scoredSessions[speakerNotified.SessionId];
						scoredSession.SpeakerNotified = true;
					},
					feedbackGiven =>
					{
						var feedback = new FeedbackData
						{
							SessionId = feedbackGiven.SessionId,
							Score = feedbackGiven.Score,
							Comment = feedbackGiven.Comment,
							Email = feedbackGiven.Email
						};
						var scoredSession = scoredSessions[feedbackGiven.SessionId];
						scoredSession.Feedback.Add(feedback);
					}
				);
			}
			return scoredSessions.Values;
		}


		public void Register_feedback_notification(string sessionId)
		{
			es.Record(new SpeakerNotified(sessionId));
		}
	}
}

using Contract.data;
using EventStore.Contract;
using Repository.data;
using Repository.events;
using System;
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

		public void Store_conference(string id, string title)
		{
			var e = new ConferenceRegistered(id, title);
			this.es.Record(e);
		}

		public int Store_sessions(string conferenceId, IEnumerable<SessionParsed> sessions)
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
				var @switch = new Dictionary<Type, Action<IRecordedEvent>>{
					{typeof(ConferenceRegistered), recordedEvent =>
					{
						var confRegistered = (ConferenceRegistered)recordedEvent.Event;
						confdata.Id = confRegistered.ConfId;
						confdata.Title = confRegistered.Title;
					}},
					{typeof(SessionAssigned), recordedEvent =>
					{
						var sessionAssigned = (SessionAssigned) recordedEvent.Event;
						sessionIds.Add(sessionAssigned.SessionId);
					}}
				};
				@switch[(e.Event.GetType())](e);
			}

			recordedEvents = this.es.QueryByType(typeof(SessionRegistered))
				.Where(e => sessionIds.Contains(e.Event.Context))
				.GroupBy(x => x.Event.Context)
				.Select(grp => grp.OrderBy(x => x.Timestamp).Last());
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
				var @switch = new Dictionary<Type, Action<IRecordedEvent>>
				{
					{typeof (ConferenceRegistered), recordedEvent =>
					{
						var confRegistered = (ConferenceRegistered) recordedEvent.Event;
						conferences.Add(confRegistered.ConfId, confRegistered.Title);
					}},
					{typeof (SessionRegistered), recordedEvent =>
					{
						var sessionRegistered = (SessionRegistered) recordedEvent.Event;
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
					}},
					{typeof (SessionAssigned), recordedEvent =>
					{
						var sessionAssigned = (SessionAssigned) recordedEvent.Event;
						var confTitle = conferences[sessionAssigned.ConfId];
						var scoredSession = scoredSessions[sessionAssigned.SessionId];
						scoredSession.ConfId = sessionAssigned.ConfId;
						scoredSession.ConfTitle = confTitle;
					}},
					{typeof (SpeakerNotified), recordedEvent =>
					{
						var speakerNotified = (SpeakerNotified) recordedEvent.Event;
						var scoredSession = scoredSessions[speakerNotified.SessionId];
						scoredSession.SpeakerNotified = true;
					}},
					{typeof (FeedbackGiven), recordedEvent =>
					{
						var feedbackGiven = (FeedbackGiven) recordedEvent.Event;
						var feedback = new FeedbackData
						{
							SessionId = feedbackGiven.SessionId,
							Score = feedbackGiven.Score,
							Comment = feedbackGiven.Comment,
							Email = feedbackGiven.Email
						};
						var scoredSession = scoredSessions[feedbackGiven.SessionId];
						scoredSession.Feedback.Add(feedback);
					}}
				};
				@switch[(e.Event.GetType())](e);
			}
			return scoredSessions.Values;
		}

		public void Register_feedback_notification(string sessionId)
		{
			es.Record(new SpeakerNotified(sessionId));
		}

		public IEnumerable<ConferenceData> Load_conferences()
		{
			var conferences = es.QueryByType(typeof (ConferenceRegistered))
				.GroupBy(x => x.Event.Context)
				.Select(grp => grp.OrderBy(x => x.Timestamp).Last());
			return conferences.Select(x =>
			{
				var confRegistered = (ConferenceRegistered) x.Event;
				return Load_conference(confRegistered.ConfId);
			});
		}

		public Session Get_Session(string sessionId)
		{
			var @event = es.QueryByType(typeof (SessionRegistered)).Single(x => x.Event.Context == sessionId).Event;
			var sessionRegistered = (SessionRegistered)@event;
			return new Session
			{
				Id	= sessionRegistered.SessionId,
				Title = sessionRegistered.Title,
				Start = sessionRegistered.Start,
				End = sessionRegistered.End,
				SpeakerName = sessionRegistered.SpeakerName
			};
		}
	}
}

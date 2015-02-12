using EventStore.Internals;
using System;

namespace Repository.events
{
	public class SessionAssigned : Event, ISemaphoreFeedbackEvent
	{
		public string ConfId;
		public string SessionId;

		public SessionAssigned(string confId, string sessionId)
			: base(confId, "SessionAssigned", sessionId)
		{
			ConfId = confId;
			SessionId = sessionId;
		}

		public SessionAssigned(string context, string name, string payload)  // deserialize
			: base(context, name, payload)
		{
			ConfId = context;
			SessionId = payload;
		}

		public void Accept(Action<ConferenceRegistered> conferenceRegistered, Action<SessionRegistered> sessionRegistered,
				   Action<SessionAssigned> sessionAssigned, Action<SpeakerNotified> speakerNotified,
				   Action<FeedbackGiven> feedbackGiven)
		{
			sessionAssigned(this);
		}
	}
}

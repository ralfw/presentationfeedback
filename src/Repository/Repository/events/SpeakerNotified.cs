using EventStore.Internals;
using System;

namespace Repository.events
{
	public class SpeakerNotified : Event, ISemaphoreFeedbackEvent
	{
		public string SessionId;

		public SpeakerNotified(string sessionId)
			: base(sessionId, "SpeakerNotified", "")
		{
			SessionId = sessionId;
		}

		public SpeakerNotified(string context, string name, string payload) // deserialize
			: base(context, name, payload)
		{
			SessionId = context;
		}

		public void Accept(Action<ConferenceRegistered> conferenceRegistered, Action<SessionRegistered> sessionRegistered,
						   Action<SessionAssigned> sessionAssigned, Action<SpeakerNotified> speakerNotified,
						   Action<FeedbackGiven> feedbackGiven)
		{
			speakerNotified(this);
		}
	}
}

using EventStore.Internals;
using System;

namespace Repository.events
{
	[Serializable]
	public class SpeakerNotified : Event, ISemaphoreFeedbackEvent
	{
		public string SessionId;

		public SpeakerNotified(string sessionId)
			: base(sessionId, "SpeakerNotified")
		{
			SessionId = sessionId;
		}

		public void Accept(Action<ConferenceRegistered> conferenceRegistered, Action<SessionRegistered> sessionRegistered,
						   Action<SessionAssigned> sessionAssigned, Action<SpeakerNotified> speakerNotified,
						   Action<FeedbackGiven> feedbackGiven)
		{
			speakerNotified(this);
		}
	}
}

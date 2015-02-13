using EventStore.Internals;
using System;

namespace Repository.events
{
	[Serializable]
	public class SessionAssigned : Event, ISemaphoreFeedbackEvent
	{
		public string ConfId;
		public string SessionId;

		public SessionAssigned(string confId, string sessionId)
			: base(confId, "SessionAssigned")
		{
			ConfId = confId;
			SessionId = sessionId;
		}

		public void Accept(Action<ConferenceRegistered> conferenceRegistered, Action<SessionRegistered> sessionRegistered,
				   Action<SessionAssigned> sessionAssigned, Action<SpeakerNotified> speakerNotified,
				   Action<FeedbackGiven> feedbackGiven)
		{
			sessionAssigned(this);
		}
	}
}

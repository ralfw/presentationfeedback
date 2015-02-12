using EventStore.Contract;
using System;

namespace Repository.events
{
	public interface ISemaphoreFeedbackEvent : IEvent
	{
		void Accept(
		Action<ConferenceRegistered> conferenceRegistered,
		Action<SessionRegistered> sessionRegistered,
		Action<SessionAssigned> sessionAssigned,
		Action<SpeakerNotified> speakerNotified,
		Action<FeedbackGiven> feedbackGiven
		);
	}
}

using EventStore.Internals;
using System;

namespace Repository.events
{
	[Serializable]
	public class ConferenceRegistered : Event, ISemaphoreFeedbackEvent
	{
		public string Title;
		public string ConfId;

		public ConferenceRegistered(string confId, string title)
			: base(confId, "ConferenceRegistered")
		{
			ConfId = confId;
			Title = title;
		}
	
		public void Accept(Action<ConferenceRegistered> conferenceRegistered, Action<SessionRegistered> sessionRegistered, 
						   Action<SessionAssigned> sessionAssigned, Action<SpeakerNotified> speakerNotified,
						   Action<FeedbackGiven> feedbackGiven)
		{
			conferenceRegistered(this);
		}
	}
}

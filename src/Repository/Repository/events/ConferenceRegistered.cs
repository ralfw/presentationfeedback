using EventStore.Internals;
using System;

namespace Repository.events
{
	public class ConferenceRegistered : Event, ISemaphoreFeedbackEvent
	{
		public string Title;
		public string ConfId;

		public ConferenceRegistered(string confId, string title)
			: base(confId, "ConferenceRegistered", title)
		{
			ConfId = confId;
			Title = title;
		}
	
		public ConferenceRegistered(string context, string name, string payload) // deserialize.
			: base(context, name, payload)
		{
			ConfId = context;
			Title = payload;
		}

		public void Accept(Action<ConferenceRegistered> conferenceRegistered, Action<SessionRegistered> sessionRegistered, 
						   Action<SessionAssigned> sessionAssigned, Action<SpeakerNotified> speakerNotified,
						   Action<FeedbackGiven> feedbackGiven)
		{
			conferenceRegistered(this);
		}
	}
}

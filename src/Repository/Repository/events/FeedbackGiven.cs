using Contract.data;
using EventStore.Internals;
using System;

namespace Repository.events
{
	[Serializable]
	public class FeedbackGiven : Event, ISemaphoreFeedbackEvent
	{
		public string SessionId;
		public TrafficLightScores Score;
		public string Comment;
		public string Email;


		public FeedbackGiven(string sessionId, TrafficLightScores score, string comment, string email)
			: base(sessionId, "FeedbackGiven")
		{
			SessionId = sessionId;
			Score = score;
			Comment = comment;
			Email = email;
		}

		public void Accept(Action<ConferenceRegistered> conferenceRegistered, Action<SessionRegistered> sessionRegistered,
				   Action<SessionAssigned> sessionAssigned, Action<SpeakerNotified> speakerNotified,
				   Action<FeedbackGiven> feedbackGiven)
		{
			feedbackGiven(this);
		}
	}
}

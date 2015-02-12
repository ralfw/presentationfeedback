using Contract.data;
using EventStore.Internals;
using System;

namespace Repository.events
{
	public class FeedbackGiven : Event, ISemaphoreFeedbackEvent
	{
		public string SessionId;
		public TrafficLightScores Score;
		public string Comment;
		public string Email;

		public FeedbackGiven(string sessionId, TrafficLightScores score, string comment, string email)
			: base(sessionId, "FeedbackGiven", string.Format("{0}\t{1}\t{2}", score, comment, email))
		{
			SessionId = sessionId;
			Score = score;
			Comment = comment;
			Email = email;
		}

		public FeedbackGiven(string sessionId, string name, string payload) // deserialize.
			: base(sessionId, name, payload)
		{
			var fields = payload.Split('\t');
			SessionId = sessionId;
			Score = (TrafficLightScores) Enum.Parse(typeof (TrafficLightScores), fields[0]);
			Comment = fields[1];
			Email = fields[2];
		}

		public void Accept(Action<ConferenceRegistered> conferenceRegistered, Action<SessionRegistered> sessionRegistered,
				   Action<SessionAssigned> sessionAssigned, Action<SpeakerNotified> speakerNotified,
				   Action<FeedbackGiven> feedbackGiven)
		{
			feedbackGiven(this);
		}
	}
}

using EventStore.Internals;
using System;

namespace Repository.events
{
	public class SessionRegistered : Event, ISemaphoreFeedbackEvent
	{
		public string SessionId;
		public string Title;
		public DateTime Start;
		public DateTime End;
		public string SpeakerName;
		public string SpeakerEmail;

		public SessionRegistered(string sessionId, string title, DateTime start, DateTime end, string speakerName, string speakerEmail) 
			: base(sessionId, "SessionRegistered", string.Format("{0}\t{1:s}\t{2:s}\t{3}\t{4}", title, start, end, speakerName, speakerEmail))
		{
			SessionId = sessionId;
			Title = title;
			Start = start;
			End = end;
			SpeakerName = speakerName;
			SpeakerEmail = speakerEmail;
		}

		public SessionRegistered(string context, string name, string payload) // deserialize
			: base(context, name, payload)
		{
			var fields = payload.Split('\t');
			SessionId = context;
			Title = fields[0];
			Start = DateTime.Parse(fields[1]);
			End = DateTime.Parse(fields[2]);
			SpeakerName = fields[3];
			SpeakerEmail = fields[4];
		}

		public void Accept(Action<ConferenceRegistered> conferenceRegistered, Action<SessionRegistered> sessionRegistered,
		   Action<SessionAssigned> sessionAssigned, Action<SpeakerNotified> speakerNotified,
		   Action<FeedbackGiven> feedbackGiven)
		{
			sessionRegistered(this);
		}
	}
}

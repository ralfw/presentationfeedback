using EventStore.Internals;
using System;

namespace Repository.events
{
	[Serializable]
	public class SessionRegistered : Event
	{
		public string SessionId;
		public string Title;
		public DateTime Start;
		public DateTime End;
		public string SpeakerName;
		public string SpeakerEmail;

		public SessionRegistered(string sessionId, string title, DateTime start, DateTime end, string speakerName, string speakerEmail) 
			: base(sessionId, "SessionRegistered")
		{
			SessionId = sessionId;
			Title = title;
			Start = start;
			End = end;
			SpeakerName = speakerName;
			SpeakerEmail = speakerEmail;
		}
	}
}

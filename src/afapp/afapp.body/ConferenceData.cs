using System;
using EventStore.Contract;
using System.Collections.Generic;

namespace afapp.body
{
	public class ConferenceData {
		public string Id;
		public string Title;
		public IList<SessionData> Sessions;
	}

	public class SessionData {
		public string Id;
		public string Title;
		public DateTime Start, End;
		public string SpeakerName;
		public string SpeakerEmail;
	}
}

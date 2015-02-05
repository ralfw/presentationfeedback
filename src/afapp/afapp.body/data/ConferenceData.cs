using System;
using System.Collections.Generic;

namespace afapp.body.data
{
	public class ConferenceData {
		public string Id;
		public string Title;
		public IEnumerable<SessionData> Sessions;

		public class SessionData {
			public string Id;
			public string Title;
			public DateTime Start, End;
			public string SpeakerName;
			public string SpeakerEmail;
		}
	}
}

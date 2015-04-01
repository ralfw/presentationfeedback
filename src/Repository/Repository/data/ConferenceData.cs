using System.Collections.Generic;

namespace Repository.data
{
	using System;
	using Contract.data;

	public class ConferenceData {
		public string Id;
		public string Title;
		public string TimeZone;
		public IEnumerable<SessionData> Sessions;

		public class SessionData {
			public string Id;
			public string Title;
			public DateTimeWithZone Start, End;
			public string SpeakerName;
			public string SpeakerEmail;
		}
	}
}

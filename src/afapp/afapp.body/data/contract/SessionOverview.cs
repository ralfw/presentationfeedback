using System;

namespace afapp.body.data.contract
{
	public struct SessionOverview {
		public string ConfId;
		public string ConfTitle;
		public Session[] ActiveSessions;
		public Session[] InactiveSessions;

		public struct Session {
			public string Id;
			public string Title;
			public DateTime Start, End;
			public string SpeakerName;
		}
	}
}

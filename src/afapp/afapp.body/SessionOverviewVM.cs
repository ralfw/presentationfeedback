using System;
using EventStore.Contract;

namespace afapp.body
{
	public struct SessionOverviewVM {
		public string ConfId;
		public string ConfTitle;
		public SessionVM[] ActiveSessions;
		public SessionVM[] InactiveSessions;
	}

	public struct SessionVM {
		public string Id;
		public string Title;
		public DateTime Start, End;
		public string SpeakerName;
	}
}

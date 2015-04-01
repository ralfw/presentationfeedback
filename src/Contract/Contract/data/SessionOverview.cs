
namespace Contract.data
{
	using System;

	public struct SessionOverview {
		public string ConfId;
		public string ConfTitle;
		public TimeZoneInfo TimeZone;
		public Session[] ActiveSessions;
		public Session[] InactiveSessions;
	}

	public struct Session {
		public string Id;
		public string Title;
		public DateTimeWithZone Start, End;
		public string SpeakerName;
	}
}

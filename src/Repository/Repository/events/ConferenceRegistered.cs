using EventStore.Internals;
using System;

namespace Repository.events
{
	[Serializable]
	public class ConferenceRegistered : Event
	{
		public string Title;
		public string ConfId;
		public string TimeZone; 

		public ConferenceRegistered(string confId, string title, string timeZone)
			: base(confId, "ConferenceRegistered")
		{
			ConfId = confId;
			Title = title;
			TimeZone = timeZone;
		}
	}
}

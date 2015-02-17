using EventStore.Internals;
using System;

namespace Repository.events
{
	[Serializable]
	public class ConferenceRegistered : Event
	{
		public string Title;
		public string ConfId;

		public ConferenceRegistered(string confId, string title)
			: base(confId, "ConferenceRegistered")
		{
			ConfId = confId;
			Title = title;
		}
	}
}

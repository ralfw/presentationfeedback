using EventStore.Internals;
using System;

namespace Repository.events
{
	[Serializable]
	public class SessionAssigned : Event
	{
		public string ConfId;
		public string SessionId;

		public SessionAssigned(string confId, string sessionId)
			: base(confId, "SessionAssigned")
		{
			ConfId = confId;
			SessionId = sessionId;
		}
	}
}

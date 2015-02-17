using EventStore.Internals;
using System;

namespace Repository.events
{
	[Serializable]
	public class SpeakerNotified : Event
	{
		public string SessionId;

		public SpeakerNotified(string sessionId)
			: base(sessionId, "SpeakerNotified")
		{
			SessionId = sessionId;
		}
	}
}

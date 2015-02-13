using EventStore.Internals;

namespace EventStore.Test
{
	using System;

	[Serializable]
	public class EventHappened: Event
	{
		public EventHappened(string context)
			: base(context, "EventHappened")
		{
			Source = context + "bar";
		}

		public string Source;
	}
}

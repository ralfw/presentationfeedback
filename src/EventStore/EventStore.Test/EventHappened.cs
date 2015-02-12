using EventStore.Internals;

namespace EventStore.Test
{
	public class EventHappened: Event
	{
		public EventHappened(string context, string name, string payload)
			: base(context, "EventHappened", "foo") { }
	}
}

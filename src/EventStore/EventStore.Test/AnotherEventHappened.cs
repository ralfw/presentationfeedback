using EventStore.Internals;

namespace EventStore.Test
{
	class AnotherEventHappened : Event
	{
		public AnotherEventHappened(string context, string name, string payload)
			: base(context, "AnotherEventHappened", "foo") { }
	}
}

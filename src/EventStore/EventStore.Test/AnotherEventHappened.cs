using EventStore.Internals;

namespace EventStore.Test
{
	using System;
	using System.Linq;

	[Serializable]
	class AnotherEventHappened : Event
	{
		public AnotherEventHappened(string context)
			: base(context, "AnotherEventHappened")
		{
			Title = context + "Foo";
			Count = context.Count();
		}

		public string Title;
		public int Count;
	}
}

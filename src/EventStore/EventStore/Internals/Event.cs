using EventStore.Contract;

namespace EventStore.Internals
{
	using System;

	[Serializable]
	public abstract class Event : IEvent
	{
		protected Event(string context, string name)
		{
			Context = context;
			Name = name;
		}

		public string Context { get; private set; }
		public string Name { get; private set; }
	}
}

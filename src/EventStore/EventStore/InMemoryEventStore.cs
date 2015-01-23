
using EventStore.Contract;
using EventStore.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStore
{
	public class InMemoryEventStore : IEventStore
	{
		private readonly IList<IRecordedEvent> events = new List<IRecordedEvent>();

		public event Action<IRecordedEvent> OnRecorded = _ => { };

		public void Record(IEvent @event)
		{
			lock (this)
			{
				var recordedEvent = new RecordedEvent(events.Count, @event);
				events.Add(recordedEvent);
				OnRecorded(recordedEvent);
			}
		}

		public IEnumerable<IRecordedEvent> Replay()
		{
			return events;
		}

		public IEnumerable<IRecordedEvent> Replay(long firstSequenceNumber)
		{
			return events.Where(x => x.SequenceNumber >= firstSequenceNumber);
		}

		public IEnumerable<IRecordedEvent> QueryByName(params string[] eventNames)
		{
			return events.Where(x => eventNames.Contains(x.Name));
		}

		public IEnumerable<IRecordedEvent> QueryByContext(params string[] contexts)
		{
			return events.Where(x => contexts.Contains(x.Context));
		}
	}
}

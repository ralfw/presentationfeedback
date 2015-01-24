﻿
using EventStore.Contract;
using EventStore.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStore
{
	using System.Threading;

	public class InMemoryEventStore : IEventStore
	{
		private readonly IList<IRecordedEvent> events = new List<IRecordedEvent>();
		private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

		public event Action<IRecordedEvent> OnRecorded = _ => { };

		public void Record(IEvent @event)
		{
			using (rwLock.Write())
			{
				var recordedEvent = new RecordedEvent(events.Count, @event);
				events.Add(recordedEvent);
				OnRecorded(recordedEvent);
			}
		}

		public IEnumerable<IRecordedEvent> Replay()
		{
			using (rwLock.Read())
			{
				return events;
			}
		}

		public IEnumerable<IRecordedEvent> Replay(long firstSequenceNumber)
		{
			return Replay().Where(x => x.SequenceNumber >= firstSequenceNumber);
		}

		public IEnumerable<IRecordedEvent> QueryByName(params string[] eventNames)
		{
			return Replay().Where(x => eventNames.Contains(x.Name));
		}

		public IEnumerable<IRecordedEvent> QueryByContext(params string[] contexts)
		{
			return Replay().Where(x => contexts.Contains(x.Context));
		}
	}
}

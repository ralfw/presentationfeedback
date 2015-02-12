using EventStore.Contract;
using System;

namespace EventStore.Internals
{
	public class RecordedEvent : IRecordedEvent
	{

		internal RecordedEvent(long sequenceNumber, IEvent @event)
			: this(Guid.NewGuid(), DateTime.Now.ToUniversalTime(), sequenceNumber, @event)
		{
		}

		internal RecordedEvent(Guid id, DateTime timestamp, long sequenceNumber, IEvent @event)
		{
			Id = id;
			Timestamp = timestamp;
			SequenceNumber = sequenceNumber;
			Event = @event;
		}

		public IEvent Event { get; private set; }
		public Guid Id { get; private set; }
		public DateTime Timestamp { get; private set; }
		public long SequenceNumber { get; private set; }
	}
}

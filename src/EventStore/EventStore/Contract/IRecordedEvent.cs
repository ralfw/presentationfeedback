
using System;

namespace EventStore.Contract
{
	public interface IRecordedEvent : IEvent
	{
		Guid Id { get; }
		DateTime Timestamp { get; }
		long SequenceNumber { get; }
	}
}

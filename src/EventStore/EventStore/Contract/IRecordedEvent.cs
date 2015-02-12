
using System;

namespace EventStore.Contract
{
	public interface IRecordedEvent
	{
		IEvent Event { get;  }
		Guid Id { get; }
		DateTime Timestamp { get; }
		long SequenceNumber { get; }
	}
}

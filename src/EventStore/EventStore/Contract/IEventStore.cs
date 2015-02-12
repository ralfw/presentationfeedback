using System;
using System.Collections.Generic;

namespace EventStore.Contract
{
	public interface IEventStore
	{
		void Record(IEvent @event);
		IEnumerable<IRecordedEvent> Replay();
		IEnumerable<IRecordedEvent> Replay(long firstSequenceNumber);
		IEnumerable<IRecordedEvent> QueryByName(params string[] eventNames);
		IEnumerable<IRecordedEvent> QueryByContext(params string[] contexts);
		IEnumerable<IRecordedEvent> QueryByType(params Type[] types);

		event Action<IRecordedEvent> OnRecorded;
	}
}

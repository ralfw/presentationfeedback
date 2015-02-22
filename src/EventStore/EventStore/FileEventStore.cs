using EventStore.Contract;
using EventStore.Internals;
using EventStore.Internals.File;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStore
{
	using System.Threading;

	public class FileEventStore : IEventStore
	{
		private readonly FileStore fileStore;
		private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

		public FileEventStore(string dirPath)
		{
			fileStore = new FileStore(dirPath);
		}

		public void Record(IEvent @event)
		{
			rwLock.Write (() => {
				var sequenceNumber = fileStore.GetNextSequenceNumber ();
				var recordedEvent = new RecordedEvent (sequenceNumber, @event);
				fileStore.Write (CreateFileName (sequenceNumber), recordedEvent);
				OnRecorded (recordedEvent);
			});
		}

		private static string CreateFileName(long sequenceNumber)
		{
			return sequenceNumber.ToString("000000000000") + ".bin";
		}

		public IEnumerable<IRecordedEvent> Replay()
		{
			IEnumerable<IRecordedEvent> snapshot = null;
			rwLock.Read (() => {
				snapshot = fileStore.ReadAll ();
			});
			return snapshot;
		}

		public IEnumerable<IRecordedEvent> Replay(long firstSequenceNumber)
		{
			return Replay().Where(x => x.SequenceNumber >= firstSequenceNumber);
		}

		public IEnumerable<IRecordedEvent> QueryByName(params string[] eventNames)
		{
			return Replay().Where(x => eventNames.Contains(x.Event.Name));
		}

		public IEnumerable<IRecordedEvent> QueryByContext(params string[] contexts)
		{
			return Replay().Where(x => contexts.Contains(x.Event.Context));
		}

		public IEnumerable<IRecordedEvent> QueryByType(params Type[] types)
		{

			return Replay().Where(x => types.Contains(x.Event.GetType()));
		}

		public event Action<IRecordedEvent> OnRecorded = _ => { };
	}
}

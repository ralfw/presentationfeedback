using EventStore.Contract;
using EventStore.Internals;
using EventStore.Internals.File;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventStore
{
	public class FileEventStore : IEventStore
	{
		private readonly FileStore fileStore;

		public FileEventStore(string dirPath)
		{
			fileStore = new FileStore(dirPath);
		}

		public void Record(IEvent @event)
		{
			lock (this)
			{
				var sequenceNumber = fileStore.GetNextSequenceNumber();
				var recordedEvent = new RecordedEvent(sequenceNumber, @event);
				fileStore.Write(CreateFileName(sequenceNumber), recordedEvent);
				OnRecorded(recordedEvent);
			}
		}

		private static string CreateFileName(long sequenceNumber)
		{
			return sequenceNumber.ToString("000000000000") + ".txt";
		}

		public IEnumerable<IRecordedEvent> Replay()
		{
			return fileStore.ReadAll();
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

		public event Action<IRecordedEvent> OnRecorded = _ => { };
	}
}

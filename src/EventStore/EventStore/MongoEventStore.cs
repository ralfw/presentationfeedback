using System.Collections.Generic;

namespace EventStore
{
	using Contract;
	using Internals;
	using MongoDB.Driver;
	using System;
	using System.Linq;
	using System.Threading;

	public class MongoEventStore : IEventStore
	{
		private readonly MongoCollection<IRecordedEvent> collection;
		private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

		public MongoEventStore(string connectionString, string databaseName)
		{
			var client = new MongoClient(connectionString);
			var server = client.GetServer();
			var database = server.GetDatabase(databaseName);
			collection = database.GetCollection<IRecordedEvent>("events");
		}

		public void Record(IEvent @event)
		{
			rwLock.Write(() =>
			{
				var recordedEvent = new RecordedEvent(collection.Count(), @event);
				collection.Insert(recordedEvent);
				OnRecorded(recordedEvent);
			});
		}

		public IEnumerable<IRecordedEvent> Replay()
		{
			IEnumerable<IRecordedEvent> snapshot = null;
			rwLock.Read(() =>
			{
				snapshot = collection.FindAll();
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

		public void Dispose()
		{

		}
	}
}

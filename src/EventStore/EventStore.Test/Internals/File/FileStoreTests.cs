using EventStore.Internals;
using EventStore.Internals.File;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace EventStore.Test.Internals.File
{
	[TestFixture]
	public class FileStoreTests
	{
		private const string DirPath = "eventStoreDir";

		[SetUp]
		public void Init()
		{
			if (Directory.Exists(DirPath))
			{
				Directory.Delete(DirPath, true);
			}
		}

		[Test]
		public void WriteAndRead()
		{
			// arrange 
			const string fileName = "fooEvent.txt";
			var testEvent = new EventHappened("session");
			var testRecordedEvent = new RecordedEvent(Guid.NewGuid(), DateTime.UtcNow, 0, testEvent);
			var sut = new FileStore(DirPath);

			// act 
			sut.Write(fileName, testRecordedEvent);
			var result = sut.ReadAll().ToList();

			// assert
			result.Count().Should().Be(1);
			result[0].Event.ShouldBeEquivalentTo(testEvent);
		}

		[Test]
		public void GetNextSequenceNumber()
		{
			// arrange 
			var sut = new FileStore(DirPath);

			// act + assert
			sut.GetNextSequenceNumber().Should().Be(0);
			sut.Write("fooEvent1.txt", new RecordedEvent(Guid.NewGuid(), DateTime.UtcNow, 0, 
				new EventHappened("session")));
			sut.GetNextSequenceNumber().Should().Be(1);
			sut.Write("fooEvent2.txt", new RecordedEvent(Guid.NewGuid(), DateTime.UtcNow, 1, 
				new AnotherEventHappened("conference")));
			sut.GetNextSequenceNumber().Should().Be(2);
		}
	}
}

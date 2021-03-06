﻿using EventStore.Contract;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EventStore.Test
{
	[TestFixture]
	public class FileEventStoreTests
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
		public void Record()
		{
			// arrange
			var testEvent0 = new AnotherEventHappened("session");
			var testEvent1 = new EventHappened("conference");
			var sut = new FileEventStore(DirPath);
			var recordedEvents = new List<IRecordedEvent>();
			sut.OnRecorded += recordedEvents.Add;

			// act 
			sut.Record(testEvent0);
			sut.Record(testEvent1);

			// assert
			recordedEvents.Count.Should().Be(2);
			recordedEvents[0].Event.ShouldBeEquivalentTo(testEvent0);
			recordedEvents[1].Event.ShouldBeEquivalentTo(testEvent1);
		}

		[Test]
		public void Replay()
		{
			// arrange 
			var testEvent0 = new AnotherEventHappened("session");
			var testEvent1 = new AnotherEventHappened("conference");
			var testEvent2 = new AnotherEventHappened("session");
			var sut = new FileEventStore(DirPath);
			sut.Record(testEvent0);
			sut.Record(testEvent1);
			sut.Record(testEvent2);

			// act  
			var result = sut.Replay().ToList();

			// assert
			result.Count().Should().Be(3);
			result[0].Event.ShouldBeEquivalentTo(testEvent0);
			result[1].Event.ShouldBeEquivalentTo(testEvent1);
		}

		[Test]
		public void Replay_FirstSequenceNumber()
		{
			// arrange
			var testEvent0 = new AnotherEventHappened("session");
			var testEvent1 = new AnotherEventHappened("conference");
			var testEvent2 = new EventHappened("session");
			var testEvent3 = new EventHappened("session");
			var sut = new FileEventStore(DirPath);
			sut.Record(testEvent0);
			sut.Record(testEvent1);
			sut.Record(testEvent2);
			sut.Record(testEvent3);

			// act  
			var result = sut.Replay(2).ToList();

			// assert
			result.Count().Should().Be(2);
			result[0].Event.ShouldBeEquivalentTo(testEvent2);
			result[1].Event.ShouldBeEquivalentTo(testEvent3);
		}

		[Test]
		public void QueryByName()
		{
			// arrange
			const string eventName1 = "EventHappened";
			var testEvent0 = new AnotherEventHappened("session");
			var testEvent1 = new EventHappened("conference");
			var testEvent2 = new EventHappened("session");
			var testEvent3 = new EventHappened("session");
			var testEvent4 = new EventHappened("conference");
			var sut = new FileEventStore(DirPath);
			sut.Record(testEvent0);
			sut.Record(testEvent1);
			sut.Record(testEvent2);
			sut.Record(testEvent3);
			sut.Record(testEvent4);

			// act  
			var result = sut.QueryByName(eventName1).ToList();

			// assert
			result.Count().Should().Be(4);
			result[0].Event.ShouldBeEquivalentTo(testEvent1);
			result[1].Event.ShouldBeEquivalentTo(testEvent2);
			result[2].Event.ShouldBeEquivalentTo(testEvent3);
			result[3].Event.ShouldBeEquivalentTo(testEvent4);
		}

		[Test]
		public void QueryByContext()
		{
			// arrange
			const string context1 = "session";
			const string context2 = "conference";
			var testEvent0 = new EventHappened("foo context");
			var testEvent1 = new AnotherEventHappened(context1);
			var testEvent2 = new EventHappened("bar context");
			var testEvent3 = new EventHappened(context2);
			var sut = new FileEventStore(DirPath);
			sut.Record(testEvent0);
			sut.Record(testEvent1);
			sut.Record(testEvent2);
			sut.Record(testEvent3);

			// act  
			var result = sut.QueryByContext(context1, context2).ToList();

			// assert
			result.Count().Should().Be(2);
			result[0].Event.ShouldBeEquivalentTo(testEvent1);
			result[1].Event.ShouldBeEquivalentTo(testEvent3);
		}

		[Test] 
		public void QueryByType()
		{
			// arrange
			const string context1 = "session";
			const string context2 = "conference";
			var testEvent0 = new EventHappened("foo context");
			var testEvent1 = new AnotherEventHappened(context1);
			var testEvent2 = new EventHappened("bar context");
			var testEvent3 = new EventHappened(context2);
			var sut = new FileEventStore(DirPath);
			sut.Record(testEvent0);
			sut.Record(testEvent1);
			sut.Record(testEvent2);
			sut.Record(testEvent3);

			// act  
			var result = sut.QueryByType(typeof(EventHappened)).ToList();

			// assert
			result.Count().Should().Be(3);
			result[0].Event.ShouldBeEquivalentTo(testEvent0);
			result[1].Event.ShouldBeEquivalentTo(testEvent2);
			result[2].Event.ShouldBeEquivalentTo(testEvent3);
		}
	}
}

using EventStore.Contract;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace EventStore.Test
{
	[TestFixture]
	public class InMemoryEventStoreTests
	{
		[Test]
		public void Record()
		{
			// arrange
			var testEvent0 = new EventHappened("session");
			var testEvent1 = new AnotherEventHappened("conference");

			var sut = new InMemoryEventStore();
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
			var testEvent0 = new EventHappened("session");
			var testEvent1 = new AnotherEventHappened("conference");
			var testEvent2 = new EventHappened("session");

			var sut = new InMemoryEventStore();
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
			var testEvent0 = new EventHappened("session");
			var testEvent1 = new AnotherEventHappened("conference");
			var testEvent2 = new AnotherEventHappened("session");
			var testEvent3 = new EventHappened("session");
			var sut = new InMemoryEventStore();
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
			const string eventName0 = "EventHappened";
			var testEvent0 = new EventHappened("session");
			var testEvent1 = new AnotherEventHappened("conference");
			var testEvent2 = new AnotherEventHappened("session");
			var testEvent3 = new AnotherEventHappened("session");
			var testEvent4 = new EventHappened("conference");
			var sut = new InMemoryEventStore();
			sut.Record(testEvent0);
			sut.Record(testEvent1);
			sut.Record(testEvent2);
			sut.Record(testEvent3);
			sut.Record(testEvent4);

			// act  
			var result = sut.QueryByName(eventName0).ToList();

			// assert
			result.Count().Should().Be(2);
			result[0].Event.ShouldBeEquivalentTo(testEvent0);
			result[1].Event.ShouldBeEquivalentTo(testEvent4);
		}

		[Test]
		public void QueryByContext()
		{
			// arrange
			const string context1 = "session";
			const string context2 = "conference";
			var testEvent0 = new EventHappened("foo context");
			var testEvent1 = new EventHappened(context1);
			var testEvent2 = new AnotherEventHappened("bar context");
			var testEvent3 = new EventHappened(context2);
			var sut = new InMemoryEventStore();
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
	}
}

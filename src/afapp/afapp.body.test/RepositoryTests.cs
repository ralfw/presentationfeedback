﻿using afapp.body.data;
using afapp.body.data.contract;


namespace afapp.body.test
{
	using EventStore.Contract;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class RepositoryTests
	{
		[Test]
		public void Store_feedback()
		{
			// arrange
			var feedbackData2 = new FeedbackData ();
			feedbackData2.SessionId = "126";
			feedbackData2.Email = "foo@bar.com";
			feedbackData2.Score = TrafficLightScores.Green;
			feedbackData2.Comment = "great!";
			var feedbackData = feedbackData2;
			var eventStoreMock = new Mock<IEventStore>();
			var target = new Repository(eventStoreMock.Object);

			// act
			target.Register_feedback(feedbackData);

			// assert
			eventStoreMock
				.Verify(x => x.Record(It.Is<IEvent>(data =>
					data.Context == feedbackData.SessionId && data.Name == "FeedbackGiven" &&
					data.Payload == string.Format("{0}\t{1}\t{2}", 
												  feedbackData.Score, feedbackData.Comment, feedbackData.Email))));
		}
	}
}

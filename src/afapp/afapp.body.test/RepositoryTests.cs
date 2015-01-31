
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
			var feedbackData = new FeedbackData
			{
				SessionId = "126",
				ConfId = "323",
				Email = "foo@bar.com",
				Score = TrafficLightScore.GREEN,
				Comment = "great!",
			};
			var eventStoreMock = new Mock<IEventStore>();
			var target = new Repository(eventStoreMock.Object);

			// act
			target.Store_feedback(feedbackData);

			// assert
			eventStoreMock
				.Verify(x => x.Record(It.Is<IEvent>(data =>
					data.Context == feedbackData.SessionId && data.Name == "FeedbackGiven" &&
					data.Payload == string.Format("{0}\t{1}\t{2}\t{3}", 
						feedbackData.ConfId, feedbackData.Email, feedbackData.Score, feedbackData.Comment))));
		}
	}
}

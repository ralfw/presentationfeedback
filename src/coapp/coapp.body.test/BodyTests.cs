
namespace coapp.body.test
{
	using Contract;
	using Contract.data;
	using Moq;
	using NUnit.Framework;
	using System;
	using System.Collections.Generic;

	[TestFixture]
	public class BodyTests
	{
		[Test]
		public void Generate_conference_overview()
		{
			// arrange
			const string confId = "c1";
			var repoMock = new Mock<ICoappRepository>();
			var expectedContent = Build_expected_content();
			var su = new Body(repoMock.Object);
			repoMock.Setup(x => x.Load_scored_sessions()).Returns(GetScoredSessionData);

			// act
			var result = su.Generate_conference_feedback(confId);

			// assert
			Assert.AreEqual("Conf 1", result.ConfTitle);
			Assert.AreEqual(expectedContent, result.Content);
		}

		private static string Build_expected_content()
		{
			return
				"Id	Title	Start	End	Speaker name	Speaker email	Greens	Yellows	Reds	Comments\n" +
				"c1s1	Session1	2015-02-08T09:00:00	2015-02-08T10:00:00	speaker 1	speaker1@mail.com	2	1	1	\"anttendee1@mail.com: Great session\n" +
					"anttendee2@mail.com: Boring session\nanttendee6@mail.com: Best session\"\n" +
				"c1s2	Session2	2015-02-08T10:00:00	2015-02-08T11:00:00	speaker 2	speaker2@mail.com	0	2	1	\"anttendee3@mail.com: Not relevant\n" +
					"anttendee1@mail.com: The sounds was too low\nanttendee5@mail.com: Not my thing\"";
		}

		private static IEnumerable<ScoredSessionData> GetScoredSessionData()
		{
			return new List<ScoredSessionData>
			{
				new ScoredSessionData
				{
					ConfId = "c1",
					ConfTitle = "Conf 1",
					Start = new DateTime(2015,02,08,09,00,00), 
					End = new DateTime(2015,02,08,10,00,00),
					SpeakerName = "speaker 1",
					SpeakerEmail = "speaker1@mail.com",
					Id = "c1s1",
					Title = "Session1",
					Feedback = new List<FeedbackData>
					{
						new FeedbackData
						{
							Email = "anttendee1@mail.com",
							Comment = "Great session",
							Score = TrafficLightScores.Green,
						},
						new FeedbackData
						{
							Email = "anttendee2@mail.com",
							Comment = "Boring session",
							Score = TrafficLightScores.Red,
						},
						new FeedbackData
						{
							Email = "anttendee3@mail.com",
							Score = TrafficLightScores.Yellow,
						},
						new FeedbackData
						{
							Email = "anttendee6@mail.com",
							Comment = "Best session",
							Score = TrafficLightScores.Green,
						}
					}
				},
				new ScoredSessionData
				{
					ConfId = "c1",
					ConfTitle = "Conf 1",
					Start = new DateTime(2015,02,08,10,00,00), 
					End = new DateTime(2015,02,08,11,00,00),
					SpeakerName = "speaker 2",
					SpeakerEmail = "speaker2@mail.com",
					Id = "c1s2",
					Title = "Session2",
					Feedback = new List<FeedbackData>
					{
						new FeedbackData
						{
							Email = "anttendee3@mail.com",
							Comment = "Not relevant",
							Score = TrafficLightScores.Red,
						},
						new FeedbackData
						{
							Email = "anttendee1@mail.com",
							Comment = "The sounds was too low",
							Score = TrafficLightScores.Yellow,
						},
						new FeedbackData
						{
							Email = "anttendee5@mail.com",
							Comment = "Not my thing",
							Score = TrafficLightScores.Yellow,
						},
					}
				},
				new ScoredSessionData
				{
					ConfId = "c2",
					ConfTitle = "Conf 2",
					Start = new DateTime(2015,03,12,10,00,00), 
					End = new DateTime(2015,03,12,11,00,00),
					SpeakerName = "speaker 3",
					SpeakerEmail = "speaker3@mail.com",
					Id = "c2s1",
					Title = "Session1",
					Feedback = new List<FeedbackData>
					{
						new FeedbackData
						{
							Email = "anttendee4@mail.com",
							Comment = "I liked the last part.",
							Score = TrafficLightScores.Green,
						},
						new FeedbackData
						{
							Email = "anttendee5@mail.com",
							Comment = "The sounds was too high",
							Score = TrafficLightScores.Yellow,
						}
					}
				}
			};
		} 
	}
}

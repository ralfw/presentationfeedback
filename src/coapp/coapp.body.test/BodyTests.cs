
using Repository.events;

namespace coapp.body.test
{
	using Contract.data;
	using NUnit.Framework;
	using System;

	[TestFixture]
	public class BodyTests
	{
		[Test]
		public void Generate_conference_overview()
		{
			// arrange
			const string timeZone = "FLE Standard Time";
			const string confId = "c1";
			var es = new EventStore.InMemoryEventStore();
			var su = new Body(es);
			var expectedContent = Build_expected_content();
			es.Record(new ConferenceRegistered("c1", "conf1", timeZone));
			es.Record(new SessionRegistered("c1s1", "Session1", new DateTime(2015, 02, 08, 09, 00, 00),
				new DateTime(2015, 02, 08, 10, 00, 00), timeZone, "speaker 1", "speaker1@mail.com"));
			es.Record(new SessionAssigned("c1", "c1s1"));
			es.Record(new SessionRegistered("c1s2", "Session2", new DateTime(2015, 02, 08, 10, 00, 00),
				new DateTime(2015, 02, 08, 11, 00, 00), timeZone, "speaker 2", "speaker2@mail.com"));
			es.Record(new SessionAssigned("c1", "c1s2"));
			es.Record(new ConferenceRegistered("c2", "conf2", timeZone));
			es.Record(new SessionRegistered("c2s1", "sess21", new DateTime(2015, 02, 08, 09, 15, 00),
				new DateTime(2015, 02, 08, 10, 15, 00), timeZone, "name3", "name3@gmail.com"));
			es.Record(new SessionAssigned("c2", "c2s1"));

			es.Record(new FeedbackGiven("c1s1", TrafficLightScores.Green, "Great session", "anttendee1@mail.com"));
			es.Record(new FeedbackGiven("c1s1", TrafficLightScores.Red, "Boring session", "anttendee2@mail.com"));
			es.Record(new FeedbackGiven("c1s1", TrafficLightScores.Green, "Best session", "anttendee6@mail.com"));
			es.Record(new FeedbackGiven("c1s1", TrafficLightScores.Yellow, "", "martin@mail.com"));

			es.Record(new FeedbackGiven("c1s2", TrafficLightScores.Red, "Not relevant", "anttendee3@mail.com"));
			es.Record(new FeedbackGiven("c1s2", TrafficLightScores.Yellow, "The sounds was too low", "anttendee1@mail.com"));
			es.Record(new FeedbackGiven("c1s2", TrafficLightScores.Yellow, "Not my thing", "anttendee5@mail.com"));

			es.Record(new FeedbackGiven("c2s1", TrafficLightScores.Green, "Super", "anttendee4@mail.com"));
			es.Record(new FeedbackGiven("c2s1", TrafficLightScores.Yellow, "", "anttendee8@mail.com"));

			// act
			var result = su.Generate_conference_feedback(confId);

			// assert
			Assert.AreEqual("conf1", result.ConfTitle);
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
	}
}

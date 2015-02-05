using afapp.body.data;
using afapp.body.data.contract;
using afapp.body.speakerNotification;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace afapp.body.test
{
	[TestFixture]
	class SpeakerNotificationHandlerTests
	{
		[Test]
		public void Run()
		{
			// arange
			const string conferenceTitle = "Conference 2015";
			const int schedulerRepeatInterval = 10;
			const int feedbackPeriod = 20;
			var now = new DateTime(2015, 1, 4, 12, 0, 0);
			var emailServiceMock = new Mock<IEmailService>();
			var dataProvider = new Mock<INotificationDataProvider>();
			var allSessions = GetAllSession(now, schedulerRepeatInterval, feedbackPeriod).ToList();
			var sut = new SpeakerNotificationHandler(emailServiceMock.Object, dataProvider.Object, new Mapper(),
				feedbackPeriod, schedulerRepeatInterval);

			TimeProvider.Configure(now);
			dataProvider.Setup(x => x.GetAllSessions()).Returns(allSessions);
			dataProvider.Setup(x => x.Get_conference_title(It.IsAny<string>())).Returns(conferenceTitle);
			dataProvider.Setup(x => x.Get_scores(allSessions[1].Id)).Returns(GetScore1());
			dataProvider.Setup(x => x.Get_scores(allSessions[2].Id)).Returns(GetScore2());

			// act
			sut.Run();

			// assert
			emailServiceMock.Verify(x => x.Send_speaker_notification(It.IsAny<SpeakerNotificationData>()), Times.Exactly(2));
			emailServiceMock.Verify(x => x.Send_speaker_notification(It.Is<SpeakerNotificationData>(data =>
				data.ConferenceTitle == conferenceTitle && data.Session == allSessions[1] && 
				data.Reds == 1 && data.Yellows == 1 && data.Greens == 4)));
			emailServiceMock.Verify(x => x.Send_speaker_notification(It.Is<SpeakerNotificationData>(data =>
				data.ConferenceTitle == conferenceTitle && data.Session == allSessions[2] &&
				data.Reds == 2 && data.Yellows == 2 && data.Greens == 1)));
		}

		private static IEnumerable<SessionData> GetAllSession(DateTime now, int workerInvocationInterval,
			int feedbackPeriod)
		{
			return new List<SessionData>
			{
				new SessionData // In the past.
				{
					Id = "1",
					Start = now.AddDays(-1).AddHours(-1),
					End = now.AddDays(-1)
				},
				new SessionData // Due. Feedback period just finished.
				{
					Id = "2",
					Start = now.AddMinutes(-(60 + feedbackPeriod)),
					End = now.AddMinutes(-(feedbackPeriod))
				},
				new SessionData // Due. Feedback period finished and we are in the middle of worker invocation interval.
				{
					Id = "3",
					Start = now.AddMinutes(-(60 + feedbackPeriod)),
					End = now.AddMinutes(-(feedbackPeriod + workerInvocationInterval /2))
				},
				new SessionData // Already processed. After feedback period and worker invocation interval.
				{
					Id = "4",
					Start = now.AddHours(-(1 + feedbackPeriod)),
					End = now.AddMinutes(-(feedbackPeriod + workerInvocationInterval))
				}
			};
		}

		private static IEnumerable<TrafficLightScores> GetScore1()
		{
			return new[]
			{
				TrafficLightScores.Red, TrafficLightScores.Green, TrafficLightScores.Yellow,
				TrafficLightScores.Green, TrafficLightScores.Green, TrafficLightScores.Green
			};
		}

		private static IEnumerable<TrafficLightScores> GetScore2()
		{
			return new[]
			{
				TrafficLightScores.Red, TrafficLightScores.Yellow, TrafficLightScores.Red,
				TrafficLightScores.Green, TrafficLightScores.Yellow
			};
		}
	}
}

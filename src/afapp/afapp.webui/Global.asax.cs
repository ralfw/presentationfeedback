using EventStore.Internals;
using MongoDB.Bson.Serialization;
using Repository.events;
using System.Web.Mvc;

namespace afapp.webui
{
	using body;
	using body.domain;
	using body.providers;
	using EventStore;
	using Providers;
	using Repository.data;
	using System;
	using System.Collections.Generic;
	using System.Web.Configuration;
	using System.Web.Routing;

	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			ControllerBuilder.Current.SetControllerFactory(new ControllerFactory());
			RegisterEventsInMongoDb();
			TimeProvider.Configure();
			Start_background_speaker_notification();
		}

		private static void RegisterEventsInMongoDb()
		{
			BsonClassMap.RegisterClassMap<RecordedEvent>();
			BsonClassMap.RegisterClassMap<ConferenceRegistered>();
			BsonClassMap.RegisterClassMap<FeedbackGiven>();
			BsonClassMap.RegisterClassMap<SessionAssigned>();
			BsonClassMap.RegisterClassMap<SessionRegistered>();
			BsonClassMap.RegisterClassMap<SpeakerNotified>();
		}

		private static void Start_background_speaker_notification()
		{
			var body = BuildBody();
			var feedbackPeriod = WebConfigurationManager.AppSettings["FeedbackPeriod"];
			var schedulerRepeatInterval = WebConfigurationManager.AppSettings["SchedulerRepeatInterval"];
			body.Start_background_speaker_notification(int.Parse(feedbackPeriod), int.Parse(schedulerRepeatInterval));
		}

		private static Body BuildBody()
		{
			var es = new MongoEventStore("mongodb://admin:admin@dogen.mongohq.com:10046/trafficlightfeedback",
				"trafficlightfeedback");
			var repo = new Repository.Repository(es);
			var conferenceFactory = new Func<ConferenceData, Conference>(data => new Conference(data));
			var scoredSessions = new Func<IEnumerable<ScoredSessionData>, ScoredSessions>(data => new ScoredSessions(data));
			var mapper = new Mapper();
			var scheduler = new SchedulingProvider();
			var notificationProvider = new EmailNotificationProvider();
			return new Body(repo, conferenceFactory, mapper, scheduler, notificationProvider, scoredSessions);
		}
	}
}

using Contract.provider;
using EventStore.Internals;
using MongoDB.Bson.Serialization;
using nsapp.body;
using nsapp.body.domain;
using nsapp.body.providers;
using Repository.events;
using System.Web.Mvc;

namespace pfapp.webui
{
	using EventStore;
	using log4net;
	using Providers;
	using Repository.data;
	using System;
	using System.Collections.Generic;
	using System.Web.Configuration;
	using System.Web.Routing;

	public class MvcApplication : System.Web.HttpApplication
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

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
			Logger.Info(string.Format("Start background speaker notification at {0} - fp: {1} = sri: {2}",
				TimeProvider.Now, feedbackPeriod, schedulerRepeatInterval));
			body.Start_background_speaker_notification(int.Parse(feedbackPeriod), int.Parse(schedulerRepeatInterval));
		}

		private static Body BuildBody()
		{
			var connString = WebConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
			var database = WebConfigurationManager.AppSettings["MongoDbDatabase"];
			var es = new MongoEventStore(connString, database);
			var repo = new Repository.Repository(es);
			var scoredSessions = new Func<IEnumerable<ScoredSessionData>, ScoredSessions>(data => new ScoredSessions(data));
			var mapper = new Mapper();
			var scheduler = new SchedulingProvider();
			var notificationProvider = new EmailNotificationProvider();
			return new Body(repo, mapper, scheduler, notificationProvider, scoredSessions);
		}
	}
}

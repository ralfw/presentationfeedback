using EventStore.Internals;
using MongoDB.Bson.Serialization;
using Repository.events;
using System.Web.Mvc;
using System.Web.Routing;

namespace afapp.webui
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			ControllerBuilder.Current.SetControllerFactory(new ControllerFactory());
			RegisterEventsInMongoDb();
		}

		private void RegisterEventsInMongoDb()
		{
			BsonClassMap.RegisterClassMap<RecordedEvent>();
			BsonClassMap.RegisterClassMap<ConferenceRegistered>();
			BsonClassMap.RegisterClassMap<FeedbackGiven>();
			BsonClassMap.RegisterClassMap<SessionAssigned>();
			BsonClassMap.RegisterClassMap<SessionRegistered>();
			BsonClassMap.RegisterClassMap<SpeakerNotified>();
		}
	}
}

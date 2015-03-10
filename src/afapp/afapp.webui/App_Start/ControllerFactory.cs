
using afapp.body;
using afapp.body.domain;
using afapp.body.providers;
using afapp.webui.Controllers;
using EventStore;
using Repository.data;
using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace afapp.webui
{
	using Providers;

	public class ControllerFactory : DefaultControllerFactory
	{

		protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
		{
			if (controllerType == typeof (HomeController))
			{
				return new HomeController();
			}
			if (controllerType == typeof (ConferenceController))
			{

				return new ConferenceController(BuildBody());
			}
			if (controllerType == typeof(FeedbackController))
			{

				return new FeedbackController(BuildBody());
			}

			throw new Exception("Unknown controller type: " + controllerType);
		}

		private static Body BuildBody()
		{
			var connString = WebConfigurationManager.AppSettings["MongoDbConn"];
			var database = WebConfigurationManager.AppSettings["MongoDbDatabase"];
			var es = new MongoEventStore(connString, database);
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
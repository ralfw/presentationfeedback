
using afapp.body;
using afapp.body.domain;
using afapp.body.providers;
using afapp.webui.Controllers;
using EventStore;
using Repository.data;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace afapp.webui
{
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
			TimeProvider.Configure();
			var es = new MongoEventStore("mongodb://admin:admin@dogen.mongohq.com:10046/trafficlightfeedback",
				"trafficlightfeedback");
			var repo = new Repository.Repository(es);
			var conferenceFactory = new Func<ConferenceData, Conference>(data => new Conference(data));
			var scoredSessions = new Func<IEnumerable<ScoredSessionData>, ScoredSessions>(data => new ScoredSessions(data));
			var mapper = new Mapper();
			var scheduler = new SchedulingProvider();
			return new Body(repo, conferenceFactory, mapper, scheduler, null, scoredSessions);
		}
	}
}
﻿using afapp.body;
using afapp.body.domain;
using EventStore;
using EventStore.Contract;
using pfapp.webui.Controllers;
using Repository.data;
using System;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace pfapp.webui
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
				return new ConferenceController(BuildAfappBody(), BuildCoappBody());
			}
			if (controllerType == typeof(FeedbackController))
			{

				return new FeedbackController(BuildAfappBody());
			}

			throw new Exception("Unknown controller type: " + controllerType);
		}

		private static Body BuildAfappBody()
		{
			var eventStore = BuildEventStore();
			var repo = new Repository.Repository(eventStore);
			var conferenceFactory = new Func<ConferenceData, Conference>(data => new Conference(data));
			var mapper = new Mapper();
			return new Body(repo, conferenceFactory, mapper);
		}

		private static coapp.body.Body BuildCoappBody()
		{
			var eventStore = BuildEventStore();
			return new coapp.body.Body(eventStore);
		}

		private static IEventStore BuildEventStore()
		{
			var connString = WebConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
			var database = WebConfigurationManager.AppSettings["MongoDbDatabase"];
			return new MongoEventStore(connString, database);
		}
	}
}
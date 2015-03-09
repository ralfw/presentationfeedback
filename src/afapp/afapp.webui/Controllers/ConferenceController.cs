﻿using afapp.body;
using System.Web.Mvc;

namespace afapp.webui.Controllers
{
	using System.Diagnostics;

	[RoutePrefix("Conference")]
	public class ConferenceController : Controller
	{
		private readonly Body body;

		public ConferenceController(Body body)
		{
			this.body = body;
		}

		[Route("{id}")]
		[HttpGet]
		public ActionResult Index(string id)
		{
			Trace.TraceInformation("Conference#Index: {0}", id);
			ViewBag.SelectedMenuItem = "Conference";
			return View(body.Generate_session_overview(id));
		}

		[Route("List")]
		[HttpGet]
		public ActionResult List()
		{
			Trace.TraceInformation("Conference#List");
			ViewBag.SelectedMenuItem = "Conference";
			return View(body.Generate_conference_overview());
		}
	}
}
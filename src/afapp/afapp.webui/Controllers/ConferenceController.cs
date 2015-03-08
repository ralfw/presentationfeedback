using afapp.body;
using afapp.body.providers;
using System;
using System.Web.Mvc;

namespace afapp.webui.Controllers
{
	public class ConferenceController : Controller
	{
		private readonly Body body;

		public ConferenceController(Body body)
		{
			this.body = body;
		}

		[HttpGet]
		public ActionResult Index(string id)
		{
			ViewBag.SelectedMenuItem = "Conference";
			TimeProvider.Configure(new DateTime(2015, 1, 23, 8, 0, 0));
			return View(body.Generate_session_overview(id));
		}

		[HttpGet]
		public ActionResult List()
		{
			ViewBag.SelectedMenuItem = "Conference";
			return View(body.Generate_conference_overview());
		}
	}
}
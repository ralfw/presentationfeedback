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
		public ActionResult Index()
		{
			return View(body.Generate_conference_overview());
		}

		[HttpGet]
		public ActionResult Show(string id)
		{
			TimeProvider.Configure(new DateTime(2015, 2, 8, 8, 0, 0));
			return View(body.Generate_session_overview(id));
		}
	}
}
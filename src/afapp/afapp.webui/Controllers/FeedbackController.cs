using afapp.body;
using System.Web.Mvc;

namespace afapp.webui.Controllers
{
	using Contract.data;

	public class FeedbackController : Controller
	{
		private readonly Body body;

		public FeedbackController(Body body)
		{
			this.body = body;
		}

		[HttpGet]
		public ActionResult Create(string confid, string id)
		{
			ViewBag.confId = confid;
			ViewBag.SelectedMenuItem = "Conference";
			return View(body.Get_Session(id));
		}

		[HttpPost]
		public ActionResult Create(string confId, string sessionId, TrafficLightScores score, string comment, string email)
		{
			body.Store_feedback(sessionId, score, comment, email);
			return RedirectToAction("Index", "Conference", new { id = confId});
		}
	}
}
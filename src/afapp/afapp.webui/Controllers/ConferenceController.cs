using System.IO;
using System.Web.Mvc;

namespace afapp.webui.Controllers
{

	[RoutePrefix("Conference")]
	public class ConferenceController : Controller
	{
		private readonly body.Body afappBody;
		private readonly coapp.body.Body coappBody;

		public ConferenceController(body.Body afappBody, coapp.body.Body coappBody)
		{
			this.afappBody = afappBody;
			this.coappBody = coappBody;
		}

		[Route("{id}")]
		[HttpGet]
		public ActionResult Index(string id)
		{
			ViewBag.SelectedMenuItem = "Conference";
			return View(afappBody.Generate_session_overview(id));
		}

		[Route("List")]
		[HttpGet]
		public ActionResult List()
		{
			ViewBag.SelectedMenuItem = "Conference";
			return View(afappBody.Generate_conference_overview());
		}

		[Route("Feedback")]
		[HttpGet]
		public ActionResult Feedback(string id)
		{
			var feedback = coappBody.Generate_conference_feedback(id);
			var memoryStream = new MemoryStream();
			var tw = new StreamWriter(memoryStream);
			tw.Write(feedback.Content);
			tw.Flush();
			tw.Close();
			return File(memoryStream.GetBuffer(), "text/plain", string.Format("{0}-feedbackResult.txt", feedback.ConfTitle));
			
		}
	}
}
using afapp.body;
using log4net;
using System.Web.Mvc;

namespace afapp.webui.Controllers
{

	[RoutePrefix("Conference")]
	public class ConferenceController : Controller
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(ConferenceController));
		private readonly Body body;

		public ConferenceController(Body body)
		{
			this.body = body;
		}

		[Route("{id}")]
		[HttpGet]
		public ActionResult Index(string id)
		{
			Logger.Info(string.Format("Conference#Index {0}", id));
			ViewBag.SelectedMenuItem = "Conference";
			return View(body.Generate_session_overview(id));
		}

		[Route("List")]
		[HttpGet]
		public ActionResult List()
		{
			Logger.Info(string.Format("Conference#List"));
			ViewBag.SelectedMenuItem = "Conference";
			return View(body.Generate_conference_overview());
		}
	}
}
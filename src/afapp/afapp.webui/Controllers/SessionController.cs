using afapp.body;
using System.Web.Mvc;

namespace afapp.webui.Controllers
{
	public class SessionController : Controller
	{
		private readonly Body body;

		public SessionController(Body body)
		{
			this.body = body;
		}

		[HttpGet]
		public ActionResult Show(string id)
		{
			return View(body.Get_Session(id));
		}
	}
}
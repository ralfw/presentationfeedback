using System.Web.Mvc;

namespace afapp.webui.Controllers
{
	public class HomeController : Controller
	{

		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}
	}
}
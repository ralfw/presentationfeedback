using System.Web.Mvc;

namespace pfapp.webui.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			ViewBag.SelectedMenuItem = "Home";
			return View();
		}

		[HttpGet]
		public ActionResult About()
		{
			ViewBag.SelectedMenuItem = "About";
			return View();
		}

		[HttpGet]
		public ActionResult Contact()
		{
			ViewBag.SelectedMenuItem = "Contact";
			return View();
		}
	}
}
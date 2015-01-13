using QuartzSpike.ViewModels;
using System.IO;
using System.Web.Mvc;

namespace QuartzSpike.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			var model = new HomeViewModel
			{
				FileContent = ReadFile()
			};
			return View(model);
		}

		private string ReadFile()
		{
			using (var streamReader = new StreamReader(Server.MapPath("/Storage/dummyData.txt")))
			{
				return streamReader.ReadLine();
			}
		}
	}
}
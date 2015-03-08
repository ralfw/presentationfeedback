using System.Web.Mvc;
using System.Web.Routing;

namespace afapp.webui
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

//			routes.MapRoute(
//				name: "BarRoute",
//				url: "{controller}/{id}",
//				defaults: new { action = "Index" },
//				constraints: new { id = @"\d+" }
//			);

//			routes.MapRoute(
//				name: "IndexSearch",
//				url: "{controller}/{id}",
//				defaults: new { action = "Index" },
//				constraints: new { action = "Index" }
//			);

//			routes.MapRoute(
//				name: "NoDetails",
//				url: "{controller}/{id}",
//				defaults: new { controller = "Appointment", action = "Details", id = UrlParameter.Optional },
//				constraints: new { id = @"^[0-9]+$" }
//			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}

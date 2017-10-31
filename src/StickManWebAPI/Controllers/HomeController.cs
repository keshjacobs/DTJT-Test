using System.Web.Mvc;

namespace StickManWebAPI.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return Redirect("Swagger");
		}
	}
}

using Microsoft.AspNetCore.Mvc;

namespace Vivantio.Samples.MvcSamplesCore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
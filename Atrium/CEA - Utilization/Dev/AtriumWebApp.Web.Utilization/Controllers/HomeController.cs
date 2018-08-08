using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Utilization.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "PELI");
            //return View(new HomeViewModel() { AccessByAppCode = (Dictionary<string, bool>)Session["userAccess"] });
        }
    }
}
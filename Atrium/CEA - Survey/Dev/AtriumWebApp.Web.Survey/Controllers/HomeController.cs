
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Survey.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "MockSurvey");
            //return View(new HomeViewModel() { AccessByAppCode = (Dictionary<string, bool>)Session["userAccess"] });
        }
    }
}

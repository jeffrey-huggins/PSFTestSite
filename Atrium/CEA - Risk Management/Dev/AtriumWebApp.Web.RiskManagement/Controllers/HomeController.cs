using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.RiskManagement.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "RMUnemployment");
            //return View(new HomeViewModel() { AccessByAppCode = (Dictionary<string, bool>)Session["userAccess"] });
        }
    }
}

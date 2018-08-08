using System.Collections.Generic;
using System.Net;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "StandardsOfCare");
            //return View(new HomeViewModel() { AccessByAppCode = (Dictionary<string, bool>)Session["userAccess"] });
        }
    }
}
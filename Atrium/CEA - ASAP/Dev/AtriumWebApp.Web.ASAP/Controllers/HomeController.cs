using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.ASAP.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.ASAP.Controllers
{
    [RestrictAccessWithApp( AppCode = "ASAP")]
    public class HomeController : BaseController
    {
        public HomeController(IOptions<AppSettingsConfig> config, ASAPHotlineContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "ASAPHotline");
        }
    }
}

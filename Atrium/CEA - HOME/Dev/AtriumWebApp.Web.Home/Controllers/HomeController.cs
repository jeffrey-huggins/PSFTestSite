using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;

namespace AtriumWebApp.Web.Home.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {

        }

        public IActionResult Index()
        {
            return View();
        }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "POR,ADR,CON,WO")]
    public class HomeController : BaseController
    {
        public HomeController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "PurchaseOrder");
        }
    }
}
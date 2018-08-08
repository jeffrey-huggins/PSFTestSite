using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Financial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "WO", Admin = true)]
    public class WriteOffAdminController : BaseAdminController
    {
        private const string AppCode = "WO";

        public WriteOffAdminController(IOptions<AppSettingsConfig> config, WriteOffContext context) : base(config, context)
        {
        }

        public IActionResult Index()
        {
            SetLookbackDaysAdmin(HttpContext, AppCode);

            var adminViewModel = CreateAdminViewModel(AppCode);

            return View(adminViewModel);
        }

        #region Other Post Backs
        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, AppCode);
            return RedirectToAction("");
        }
        #endregion
    }
}
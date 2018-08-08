using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "ADR", Admin = true)]
    public class ADRAdminController : BaseAdminController
    {
        private const string AppCode = "ADR";
        public ADRAdminController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
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
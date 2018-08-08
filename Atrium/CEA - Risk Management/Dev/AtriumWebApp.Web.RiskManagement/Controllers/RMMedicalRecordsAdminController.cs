using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.RiskManagement.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "RMM", Admin = true)]
    public class RMMedicalRecordsAdminController : BaseAdminController
    {
        private const string AppCode = "RMM";

        public RMMedicalRecordsAdminController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            //If user doesn't have access, redirect to the home page
            var redirectToAction = DetermineWebpageAccess(AppCode);

            if (redirectToAction != null) return redirectToAction;
            SetLookbackDaysAdmin(HttpContext, AppCode);
            var adminViewModel = CreateAdminViewModel(AppCode);

            return View(adminViewModel);
        }

        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, AppCode);
            return RedirectToAction("");
        }
    }
}

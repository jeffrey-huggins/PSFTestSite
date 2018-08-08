using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Utilization.Models;
using AtriumWebApp.Web.Utilization.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Utilization.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "PASRR", Admin = true)]
    public class PASRRAdminController : BaseAdminController
    {
        private const string AppCode = "PASRR";

        public PASRRAdminController(IOptions<AppSettingsConfig> config, PASRRContext context) : base(config, context)
        {
        }

        protected new PASRRContext Context
        {
            get { return (PASRRContext)base.Context; }
        }

        public ActionResult Index()
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }

            SetLookbackDaysAdmin(HttpContext, AppCode);

            return View(new PASRRAdminViewModel { AdminViewModel = CreateAdminViewModel(AppCode) });
        }

        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, AppCode);
            return RedirectToAction("");
        }
    }
}

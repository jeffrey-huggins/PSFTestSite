using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "EVAC",Admin = true)]
    public class EmployeeVaccinationAdminController : BaseAdminController
    {
        private const string AppCode = "EVAC";

        public EmployeeVaccinationAdminController(IOptions<AppSettingsConfig> config, VaccinationContext context) : base(config, context)
        {
        }

        protected new VaccinationContext Context
        {
            get { return (VaccinationContext)base.Context; }
        }

        public IActionResult Index()
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }

            SetLookbackDaysAdmin(HttpContext, AppCode);
            var adminViewModel = CreateAdminViewModel(AppCode);

            return View(new EmployeeVaccinationAdminViewModel
            {
                AdminViewModel = adminViewModel
            });
        }
        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, AppCode);
            return RedirectToAction("");
        }
    }
}
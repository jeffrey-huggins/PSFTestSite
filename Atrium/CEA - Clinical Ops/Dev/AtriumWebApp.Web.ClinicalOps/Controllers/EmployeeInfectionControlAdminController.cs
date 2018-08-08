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
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "EIFC", Admin =true)]
    public class EmployeeInfectionControlAdminController : BaseAdminController
    {
        private const string AppCode = "EIFC";

        public EmployeeInfectionControlAdminController(IOptions<AppSettingsConfig> config, InfectionControlContext context) : base(config, context)
        {
        }

        protected new InfectionControlContext Context
        {
            get { return (InfectionControlContext)base.Context; }
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

            return View(new EmployeeInfectionControlAdminViewModel
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
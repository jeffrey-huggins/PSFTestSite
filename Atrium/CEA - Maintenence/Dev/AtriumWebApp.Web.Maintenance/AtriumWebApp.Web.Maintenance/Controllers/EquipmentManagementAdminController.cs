using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Maintenance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AtriumWebApp.Web.Maintenance.Models.ViewModels;

namespace AtriumWebApp.Web.Maintenance.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "EQMAN", Admin = true)]
    public class EquipmentManagementAdminController : BaseAdminController
    {
        private const string AppCode = "EQMAN";
        public EquipmentManagementAdminController(IOptions<AppSettingsConfig> config, EquipmentManagementContext context) : base(config, context)
        {
        }

        protected new EquipmentManagementContext Context
        {
            get { return (EquipmentManagementContext)base.Context; }
        }

        public IActionResult Index()
        {
            
            var adminViewModel = CreateAdminViewModel(AppCode);
            
            return View(new EquipmentManagementAdminViewModel
            {
                AdminViewModel = adminViewModel,
                LookbackDays = Context.Applications.First(a => a.ApplicationCode == AppCode).LookbackDays
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
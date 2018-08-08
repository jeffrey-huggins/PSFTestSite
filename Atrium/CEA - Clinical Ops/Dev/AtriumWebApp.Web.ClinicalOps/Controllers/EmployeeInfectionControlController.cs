using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "EIFC")]
    public class EmployeeInfectionControlController : BaseController
    {
        private const string AppCode = "EIFC";

        public EmployeeInfectionControlController(IOptions<AppSettingsConfig> config, InfectionControlContext context) : base(config, context)
        {
        }

        protected new InfectionControlContext Context
        {
            get { return (InfectionControlContext)base.Context; }
        }

        public ActionResult Index()
        {
            //Record user access
            LogSession(AppCode);
            //Set Census Date Information and Manipulate when changed
            SetLookbackDays(HttpContext, AppCode);
            //Set initial date range values
            SetInitialTableRange(AppCode);

            if (!Session.TryGetObject("EmployeeSideBar", out EmployeeSidebarViewModel sideBar))
            {
                sideBar = SideBarService.InitEmployeeSideBar(this, AppCode, Context);
            }

            EmployeeInfectionControlViewModel vm = new EmployeeInfectionControlViewModel()
            {
                SideBar = sideBar,
                RangeTo = OccurredRangeTo[AppCode],
                RangeFrom = OccurredRangeFrom[AppCode]
            };
            return View(vm);
        }

        public IActionResult GetInfectionList(int employeeId, string fromString, string toString)
        {
            DateTime from = Convert.ToDateTime(fromString);
            DateTime to = Convert.ToDateTime(toString).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<EmployeeIFCEvent> events = Context.EmployeeIFCEvents
                .Include("Site")
                .Where(a => a.EmployeeId == employeeId && a.OnsetDate >= from && a.OnsetDate <= to && !a.DeletedFlg).ToList();
            return PartialView(events);
        }

        public IActionResult EditOrCreateInfection(int? infectionId, int employeeId)
        {
            ViewBag.InfectionSites = Context.Sites.Where(a => a.DataEntryFlg)
                .OrderBy(a => a.SortOrder)
                .ThenBy(a => a.PatientIFCSiteName).ToList();
            EmployeeIFCEvent infectionEvent = new EmployeeIFCEvent();
            if (infectionId.HasValue)
            {
                infectionEvent = Context.EmployeeIFCEvents.Find(infectionId.Value);
            }
            else
            {
                infectionEvent.EmployeeId = employeeId;
            }

            return EditorFor(infectionEvent);
        }

        #region Save New/Edit

        public ActionResult SaveInfection(EmployeeIFCEvent form)
        {
            if(form.EmployeeIFCEventId == 0)
            {
                Context.EmployeeIFCEvents.Add(form);
            }
            else
            {
                EmployeeIFCEvent current = Context.EmployeeIFCEvents.Find(form.EmployeeIFCEventId);
                Context.Entry(current).CurrentValues.SetValues(form);
                
            }
            Context.SaveChanges();

            return Json(new { success = true });
        }
        #endregion


        #region Ajax Calls




        public JsonResult DeleteInfection(int infectionId)
        {
            var ifc = Context.EmployeeIFCEvents.Find(infectionId);
            var success = false;

            if (ifc != null)
            {
                ifc.DeletedFlg = true;
                ifc.DeletedTS = DateTime.Now;
                ifc.DeletedADDomainName = User.Identity.Name;
                try
                {
                    Context.SaveChanges();
                    success = true;
                }
                catch
                {
                    success = false;
                }
            }

            return Json(new SaveResultViewModel { Success = success });
        }

        #endregion
    }
}
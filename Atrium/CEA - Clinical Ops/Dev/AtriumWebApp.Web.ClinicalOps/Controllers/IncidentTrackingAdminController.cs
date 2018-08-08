using System.Linq;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "ITR")]
    public class IncidentTrackingAdminController : BaseAdminController
    {
        private const string AppCode = "ITR";

        public IncidentTrackingAdminController(IOptions<AppSettingsConfig> config, IncidentTrackingContext context) : base(config, context)
        {
        }

        protected new IncidentTrackingContext Context
        {
            get { return (IncidentTrackingContext)base.Context; }
        }

        public ActionResult Index()
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            SetLookbackDaysAdmin(HttpContext, AppCode);
            var adminViewModel = CreateAdminViewModel(AppCode);
            return View(new IncidentTrackingAdminViewModel
            {
                PayerGroups = Context.AtriumPayerGroups.ToList(),
                CommunityPayerGroupInfo = Context.PayerGroupInfos.Where(a => a.ApplicationId == adminViewModel.AppId).ToList(),
                Locations = Context.IncidentLocations.ToList(),
                Treatments = Context.IncidentTreatments.ToList(),
                Interventions = Context.IncidentInterventions.ToList(),
                IncidentTypes = Context.PatientIncidentTypes.ToList(),
                RegionalNurseForCommunity = Context.RegionalNurses.ToList(),
                RegionalNurses = Context.Employees.Where(e => e.JobClasses.Any(a => a.JobClass.JobDescription == "Other Nursing Admin") && e.EmployeeStatus == "Active").ToList(),
                CloseAllCommmunityEmployees = Context.Employees.Where(e => (e.JobClasses.Any(j => j.JobClass.JobDescription == "Other Nursing Admin" || j.JobClass.JobDescription == "Administrative Executive")) && e.EmployeeStatus == "Active").ToList(),
                CurrentCloseAllCommunityEmployees = Context.CloseAllCommunitiesEmployees.ToList(),
                AdminViewModel = adminViewModel
            });
        }

        #region Create New Types
        [HttpPost]
        public ActionResult NewType(PatientIncidentType NewIncidentType)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.PatientIncidentTypes.Add(NewIncidentType);
            Context.SaveChanges();
            return Json(new { Success = true, data = NewIncidentType });
        }

        [HttpPost]
        public ActionResult NewLocation(PatientIncidentLocation NewLocation)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.IncidentLocations.Add(NewLocation);
            Context.SaveChanges();
            return Json(new { Success = true, data = NewLocation });
        }

        [HttpPost]
        public ActionResult NewIntervention(IncidentIntervention NewIntervention)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.IncidentInterventions.Add(NewIntervention);
            Context.SaveChanges();
            return Json(new { Success = true, data = NewIntervention });
        }

        [HttpPost]
        public ActionResult NewTreatment(IncidentTreatment NewTreatment)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.IncidentTreatments.Add(NewTreatment);
            Context.SaveChanges();
            return Json(new { Success = true, data = NewTreatment });
        }
        #endregion

        #region Other Post Backs
        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, "ITR");
            return RedirectToAction("");
        }
        #endregion

        #region Ajax Calls
        public JsonResult ChangeRegionalNurse(int nurseId, int community, bool rFlag)
        {
            try
            {
                if (rFlag)
                {
                    var newRegionalNurse = new RegionalNurseCommunityInfo
                    {
                        CommunityId = community,
                        RegionalNurseEmployeeId = nurseId
                    };
                    Context.RegionalNurses.Add(newRegionalNurse);
                }
                else
                {
                    Context.RegionalNurses.Remove(Context.RegionalNurses.Single(r => r.RegionalNurseEmployeeId == nurseId && r.CommunityId == community));
                }
                Context.SaveChanges();
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            var nurses = "";
            foreach (var rNurse in Context.RegionalNurses.Where(r => r.CommunityId == community).ToList())
            {
                nurses += rNurse.RegionalNurseEmployeeId + "|";
            }
            return Json(new { Success = true, Nurses = nurses });
        }

        public JsonResult ChangeCloseAll(int employeeId, bool cFlag)
        {
            try
            {
                if (cFlag)
                {
                    var employeeClose = new CloseIncidentAllCommunity
                    {
                        EmployeeId = employeeId
                    };
                    Context.CloseAllCommunitiesEmployees.Add(employeeClose);
                }
                else
                {
                    Context.CloseAllCommunitiesEmployees.Remove(Context.CloseAllCommunitiesEmployees.Find(employeeId));
                }
                Context.SaveChanges();
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion
    }
}
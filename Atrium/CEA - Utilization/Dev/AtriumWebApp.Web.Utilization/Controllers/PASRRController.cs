using System;
using System.Collections.Generic;
using System.Linq;

using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;

using AtriumWebApp.Web.Utilization.Models;
using AtriumWebApp.Web.Utilization.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Utilization.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "PASRR")]
    public class PASRRController : BaseController
    {
        private const string AppCode = "PASRR";

        public PASRRController(IOptions<AppSettingsConfig> config, PASRRContext context) : base(config, context)
        {
        }

        protected new PASRRContext Context
        {
            get { return (PASRRContext)base.Context; }
        }


        public ActionResult Index()
        {
            //Record user access
            LogSession(AppCode);

            //Set Census Date Information and Manipulate when changed
            InitializeCensusDateChangedSessionVariable();
            SetLookbackDays(HttpContext, AppCode);
            ManipulateCensusDate(AppCode);

            //Set initial date range values
            SetDateRangeErrorValues();
            SetInitialTableRange(AppCode);

            //Set Community Drop Down based on user access privileges
            GetCommunitiesDropDownWithFilter(AppCode);
            ViewData["Communities"] = new SelectList(FacilityList[AppCode], "CommunityId", "CommunityShortName");
            var facilityId = CurrentFacility[AppCode];
            var facility = Context.Facilities.Single(f => f.CommunityId == facilityId);
            Session.SetItem(AppCode + "CurrentFacilityStateCode", facility.StateCd);

            Session.TryGetObject(AppCode + "ResidentsList",out List<Patient> patientList);
            ViewData["Residents"] = new SelectList(patientList.Select(a => new SelectListItem {
                Text = a.LastName + ", " + a.FirstName,
                Value = a.PatientId.ToString()
            }), "Value", "Text");

            // Get Vaccination recoreds for current resident
            if (Session.Contains(AppCode + "CurrentResidentId"))
            {
                Session.TryGetObject(AppCode + "CurrentResidentId",out int residentId);

                var patientRecords = Context.PatientPASRRLogs
                    .Include("PASRRType")
                    .Include("SigChangeType")
                    .Where(p => p.PatientId == residentId)
                    .ToList();

                var pasrrTypes = Context.PASRRTypes
                    .Include("StateCodes")
                    .Where(p => p.DataEntryFlg && p.StateCodes.Any(s => s.StateCode == facility.StateCd))
                    .OrderBy(p => p.SortOrder)
                    .ThenBy(p => p.PASRRTypeId)//.ThenBy(p => p.PASRRTypeName)
                    .ToList();

                var sigChangeTypes = Context.SigChangeTypes
                    .Include("StateCodes")
                    .Where(p => p.DataEntryFlg && p.StateCodes.Any(s => s.StateCode == facility.StateCd))
                    .OrderBy(p => p.SortOrder)
                    .ThenBy(p => p.SigChangeTypeName)
                    .ToList();

                return View(new PASRRViewModel {
                    PatientPASRRLogs = patientRecords,
                    PASRRTypes = pasrrTypes,
                    SigChangeTypes = sigChangeTypes
                });
            }

            return View();
        }

        #region Sidebar Post Backs
        [HttpPost] //Side Drop Down List update
        public ActionResult SideDDL(string residents, string returnUrl)
        {
            var residentId = 0;
            if (!Int32.TryParse(residents, out residentId))
            {
                return Redirect(returnUrl);
            }
            Session.SetItem(AppCode + "CurrentResidentIdString", residents);
            GetAllResidentInformation(residentId, AppCode);
            return Redirect(returnUrl);
        }

        [HttpPost]
        public ActionResult Lookback(string lookbackDate, string returnUrl, string CurrentCommunity)
        {
            return SaveLookbackDate(lookbackDate, returnUrl, CurrentCommunity, AppCode);
        }

        [HttpPost]
        public ActionResult UpdateRange(string occurredRangeFrom, string occurredRangeTo, string returnUrl)
        {
            return UpdateTableRange(occurredRangeFrom, occurredRangeTo, returnUrl, AppCode);
        }
        #endregion

        #region Ajax Calls
        public ActionResult GetPASRRLog(int pasrrLogId)
        {
            PatientPASRRLog record = Context.PatientPASRRLogs.Find(pasrrLogId);
            if (record == null)
            {
                return new NotFoundResult();
            }
            return Json(record);
        }

        public ActionResult DeletePASRRLog(int pasrrLogId)
        {
            var record = Context.PatientPASRRLogs.Find(pasrrLogId);
            Context.PatientPASRRLogs.Remove(record);
            Context.SaveChanges();
            return new OkResult();
        }

        [HttpPost]
        public ActionResult SavePASRRLog(IFormCollection form)
        {
            var pasrrLogId = form["PASRRLogId"];
            PatientPASRRLog record;

            if (String.IsNullOrWhiteSpace(pasrrLogId))
            {
                Session.TryGetObject(AppCode + "CurrentResidentId", out int patientId);
                record = new PatientPASRRLog
                {
                    PatientId = patientId,
                    PASRRTypeId = Int32.Parse(form["PASRRTypeId"]),
                    InsertedDate = DateTime.Now
                };

                Context.PatientPASRRLogs.Add(record);
            }
            else
            {
                record = Context.PatientPASRRLogs.Find(Int32.Parse(pasrrLogId));
                record.LastModifiedDate = DateTime.Now;
            }

            record.CompleteDate = DateTime.Parse(form["CompleteDate"]);
            record.SigChangeTypeId = !String.IsNullOrWhiteSpace(form["SigChangeType"]) ? Int32.Parse(form["SigChangeType"]) : -1;
            record.HospitalExemption = form["HospitalExemption"].Contains("true");
            record.HospitalExemptionExpirationDate = !String.IsNullOrWhiteSpace(form["HospitalExemptionExpirationDate"]) ? DateTime.Parse(form["HospitalExemptionExpirationDate"]) : (DateTime?)null;
            record.StayGreaterThan30Days = form["StayGreaterThan30Days"].Contains("true");
            record.DementiaExemption = form["DementiaExemption"].Contains("true");
            record.LevelIINeeded = form["LevelIINeeded"].Contains("true");
            record.LevelIIRequestedDate = !String.IsNullOrWhiteSpace(form["LevelIIRequestedDate"]) ? DateTime.Parse(form["LevelIIRequestedDate"]) : (DateTime?)null;
            record.LevelIICompletedDate = !String.IsNullOrWhiteSpace(form["LevelIICompletedDate"]) ? DateTime.Parse(form["LevelIICompletedDate"]) : (DateTime?)null;

            Context.SaveChanges();

            // Fill Navigation Properties
            if (record.PASRRType == null)
            {
                record.PASRRType = Context.PASRRTypes.Find(record.PASRRTypeId);
            }
            if (record.SigChangeType == null)
            {
                record.SigChangeType = Context.SigChangeTypes.Find(record.SigChangeTypeId);
            }

            return Json(record);
        }
        #endregion

        #region Private Helper Functions

        #endregion
    }
}

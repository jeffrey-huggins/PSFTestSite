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
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "PELI")]
    public class PELIController : BaseController
    {
        private const string AppCode = "PELI";

        public PELIController(IOptions<AppSettingsConfig> config, PELIContext context) : base(config, context)
        {
        }

        protected new PELIContext Context
        {
            get { return (PELIContext)base.Context; }
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
            Session.TryGetObject(AppCode + "ResidentsList", out List<Patient> patientList);
            ViewData["Residents"] = new SelectList(patientList.Select(a => new SelectListItem {
                Text = a.LastName + ", " + a.FirstName,
                Value = a.PatientId.ToString()
            }), "Value", "Text");

            // Get PELI Records for current resident
            if (Session.Contains(AppCode + "CurrentResidentId"))
            {
                Session.TryGetObject(AppCode + "CurrentResidentId", out int residentId);

                List<PELIType> _PELITypesAll = GetALLPELITypeList();

                List<PatientPELILog> patientRecords = Context.PatientPELILogs
                    .Where(p => p.PatientId == residentId &&
                        p.DeletedFlg == false)
                    .OrderByDescending(p => p.AdmitDate)
                    .ThenByDescending(p => p.InsertedDate)
                    .ToList();
                patientRecords.Select(e => e.PELIType = _PELITypesAll);
                
                //foreach(PatientPELILog patient in patientRecords) {
                //     string typeDesc = _PELITypes
                //         .Where(t => t.Id == patient.PELITypeId)
                //         .Select(t => t.Description).FirstOrDefault();
                //     patient.PELITypeDESC = typeDesc;
                //}

                PELIViewModel peli = new PELIViewModel
                {
                    PELITypes = GetPELITypeList(),
                    PELITypesALL = _PELITypesAll
                };
                if (patientRecords != null && patientRecords.Count > 0)
                    peli.PatientPELILogs = patientRecords;

                return View(peli);
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
        public JsonResult GetPELITypeDD(int PELITypeId)
        {
            List<SelectListItem> output = new List<SelectListItem>();
            foreach (PELIType pt in GetPELITypeList())
                output.Add(new SelectListItem { Value= pt.Id.ToString(), Text = pt.Description });
            return Json(output);
        }

        public ActionResult GetPELILog(int PELILogId)
        {
            PatientPELILog record = Context.PatientPELILogs.Find(PELILogId);
            if (record == null)
            {
                return new NotFoundResult();
            }
            return Json(record);
        }

        public void DeletePELILog(int PELILogId)
        {
            var record = Context.PatientPELILogs.Find(PELILogId);
            Context.PatientPELILogs.Remove(record);
            Context.SaveChanges();
        }

        [HttpPost]
        public ActionResult SavePELILog(IFormCollection form)
        {
            var PELILogId = form["PELILogId"];
            PatientPELILog record;

            if (String.IsNullOrWhiteSpace(PELILogId))
            {
                Session.TryGetObject(AppCode + "CurrentResidentId", out int patientId);
                record = new PatientPELILog
                {
                    PatientId = patientId,
                    PELITypeId = Int32.Parse(form["PELITypeId"]),
                    InsertedDate = DateTime.Now
                };

                Context.PatientPELILogs.Add(record);
            }
            else
            {
                record = Context.PatientPELILogs.Find(Int32.Parse(PELILogId));
                record.LastModifiedDate = DateTime.Now;
            }

            if (!String.IsNullOrWhiteSpace(form["AdmitDate"]))
                record.AdmitDate = DateTime.Parse(form["AdmitDate"]);
            if (!String.IsNullOrWhiteSpace(form["CompletedDate"]))
                record.CompletedDate = DateTime.Parse(form["CompletedDate"]);
            else
                record.CompletedDate = null;
            if(!String.IsNullOrWhiteSpace(form["PELITypeId"]))
                record.PELITypeId = Int32.Parse(form["PELITypeId"].First().Substring(form["PELITypeId"].First().IndexOf(',')+1));

            Context.SaveChanges();

            if (record.PELIType == null)
                record.PELIType = GetPELITypeList();

            return Json(record);
        }
        #endregion

        #region Private Helper Functions

        private List<PELIType> GetPELITypeList()
        {
            return Context.PELITypes
                    .Where(pt => pt.DataEntryFlg == true)
                    .OrderBy(p => p.SortOrder)
                    .ThenBy(p => p.Description)
                    .ToList();
        }

        public List<PELIType> GetALLPELITypeList()
        {
            return Context.PELITypes
                .Where(pt => pt.Id > -1)
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Description)
                .ToList();
        }

        public int NextSortOrder()
        {
            return Context.PELITypes.Count();
        }
        #endregion
    }
}

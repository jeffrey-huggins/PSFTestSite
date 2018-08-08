using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Utilization.Models;
using AtriumWebApp.Web.Utilization.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Utilization.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "SKC")]
    public class SkilledChartingController : BaseController
    {
        private const string AppCode = "SKC";

        public SkilledChartingController(IOptions<AppSettingsConfig> config, SkilledChartingContext context) : base(config, context)
        {
        }

        protected new SkilledChartingContext Context
        {
            get { return (SkilledChartingContext)base.Context; }
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
            Session.TryGetObject(AppCode + "ResidentsList", out List<Patient> patientList);
            ViewData["Residents"] = new SelectList(patientList.Select(a => new SelectListItem {
                Text = a.LastName + ", " + a.FirstName,
                Value = a.PatientId.ToString()
            }), "Value", "Text");

            // Get Vaccination recoreds for current resident
            if (Session.Contains(AppCode + "CurrentResidentId"))
            {
                Session.TryGetObject(AppCode + "CurrentResidentId", out int residentId);

                var patientRecords = Context.PatientSkilledChartings
                    .Include("DocumentationQueue")
                    .Where(s => s.PatientId == residentId)
                    .ToList();

                var customRecords = Context.PatientSkilledChartingCustoms
                    .Where(s => s.PatientId == residentId)
                    .ToList();

                var guidelines = Context.SkilledChartingGuidelines
                    .OrderBy(g => g.SortOrder)
                    .ThenBy(g => g.GuidelineName)
                    .ToList();

                guidelines.Add(GetCustomGuideline());

                return View(new SkilledChartingViewModel {
                    PatientSkilledChartingRecords = patientRecords,
                    PatientSkilledChartingCustomRecords = customRecords,
                    SkilledChartingGuidlines = guidelines
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

        //[HttpPost]
        //public ActionResult UpdateRange(string occurredRangeFrom, string occurredRangeTo, string returnUrl)
        //{
        //    return UpdateTableRange(occurredRangeFrom, occurredRangeTo, returnUrl, AppCode);
        //}

        //public ActionResult Resident(int facilityId)
        //{
        //    return GetResidentListForCommunity(facilityId, AppCode);
        //}
        #endregion

        #region Ajax Calls
        [HttpPost]
        public ActionResult SaveSkilledCharting(int patientId, int guidelineId, List<int> documentationQueues)
        {
            var patientRecords = new List<PatientSkilledCharting>();

            using (TransactionScope scope = new TransactionScope())
            {
                ClearPatientGuideline(patientId, guidelineId);

                foreach (var queue in documentationQueues)
                {
                    var rec = new PatientSkilledCharting { 
                        PatientId = patientId,
                        DocumentationQueueId = queue
                    };
                    patientRecords.Add(rec);
                    Context.PatientSkilledChartings.Add(rec);
                }

                Context.SaveChanges();
                scope.Complete();
            }

            return Json(new {
                Guidline = Context.SkilledChartingGuidelines.Find(guidelineId),
                DocumentationQueues = documentationQueues
            });
        }

        [HttpPost]
        public ActionResult SaveSkilledChartingCustom(int patientId, List<string> documentationQueues)
        {
            var patientRecords = new List<PatientSkilledChartingCustom>();

            using (TransactionScope scope = new TransactionScope())
            {
                ClearPatientCustomGuideline(patientId);

                foreach (var queue in documentationQueues)
                {
                    var rec = new PatientSkilledChartingCustom {
                        PatientId = patientId,
                        CustomQueueId = patientRecords.Count + 1,
                        CustomQueueText = queue
                    };
                    patientRecords.Add(rec);
                    Context.PatientSkilledChartingCustoms.Add(rec);
                }

                Context.SaveChanges();
                scope.Complete();
            }

            return Json(new {
                Guidline = GetCustomGuideline(),
                DocumentationQueues = string.Join(";;", documentationQueues)
            });
        }

        [HttpPost]
        public ActionResult DeleteSkilledCharting(int patientId, int guidelineId)
        {
            if (guidelineId != -1)
            {
                ClearPatientGuideline(patientId, guidelineId);
            }
            else
            {
                ClearPatientCustomGuideline(patientId);
            }
            return new StatusCodeResult(200);
        }
        #endregion

        #region Private Helper Functions
        private void ClearPatientGuideline(int patientId, int guidelineId)
        {
            var recs = Context.PatientSkilledChartings
                .Where(s => s.PatientId == patientId && s.DocumentationQueue.GuidelineId == guidelineId)
                .ToList();

            foreach (var rec in recs)
            {
                Context.PatientSkilledChartings.Remove(rec);
            }

            Context.SaveChanges();
        }

        private void ClearPatientCustomGuideline(int patientId)
        {
            var recs = Context.PatientSkilledChartingCustoms
                .Where(s => s.PatientId == patientId)
                .ToList();

            foreach (var rec in recs)
            {
                Context.PatientSkilledChartingCustoms.Remove(rec);
            }

            Context.SaveChanges();
        }

        private SkilledChartingGuideline GetCustomGuideline()
        {
            return new SkilledChartingGuideline()
            {
                GuidelineId = -1,
                GuidelineName = "Custom Guideline",
                SortOrder = 9999,
                DataEntryFlg = true
            };
        }
        #endregion
    }
}

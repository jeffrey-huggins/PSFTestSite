using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.RiskManagement.Models;
using AtriumWebApp.Web.RiskManagement.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.RiskManagement.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "RMW")]
    public class RMWorkerCompController : BaseRiskManagementController
    {
        public const string AppCode = "RMW";

        public RMWorkerCompController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        public DateTime RangeFrom
        {
            get { return DateTime.Parse(OccurredRangeFrom[AppCode]); }
        }

        public DateTime RangeTo
        {
            get { return DateTime.Parse(OccurredRangeTo[AppCode]); }
        }

        public ActionResult Index()
        {
            if (!Session.TryGetObject(AppCode + "CurrentEmployeeClaimId",out string claimId))
            {
                return RedirectToAction("CurrentClaims");
            }
            //Set Community Drop Down based on user access privileges (Employee)
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            ViewData["Communities"] = new SelectList(FacilityList[AppCode], "CommunityId", "CommunityShortName");
            Session.TryGetObject(AppCode + "Employees", out List<Employee> employeeListFull);

            var employeesFull = new SelectList(employeeListFull.Select(a => new SelectListItem()
            {
                Text = a.LastName + ", " + a.FirstName,
                Value = a.EmployeeId.ToString()
            }), "Value", "Text");
            ViewData["Employees"] = employeesFull;
            //Set drop down menus for all types
            SetWorkersCompInsuranceDropDown();
            SetWorkersCompClaimTypeDropDown();
            return RMWCurrentEmployeeClaim();
        }

        public ActionResult CurrentClaims()
        {
            LogSession(AppCode);
            SetDateRangeErrorValues();
            SetLookbackDays(HttpContext, AppCode);
            SetInitialTableRangeLookback(AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            ViewData["Communities"] = new SelectList(FacilityList[AppCode], "CommunityId", "CommunityShortName");
            using (var context = new WorkersCompContext())
            {
                var rangeFromDate = RangeFrom;
                var rangeToDate = RangeTo;
                var compClaims = context.WorkersCompClaims
                    .Where(w => w.ReportedtoCarrierDate >= rangeFromDate && w.ReportedtoCarrierDate <= rangeToDate)
                    .Select(w => new CurrentClaimViewModel
                    {
                        ClaimId = w.ClaimId,
                        CommunityId = w.Employee.CommunityId,
                        ReportedToCarrierDate = w.ReportedtoCarrierDate,
                        FirstName = w.Employee.FirstName,
                        LastName = w.Employee.LastName,
                        MaskedSocialSecurityNumber = "XXX-XX-" + w.Employee.SocialSecurityNumber.Substring(5),
                        BirthDate = w.Employee.BirthDate
                    });
                var compCliamsEnumerated = compClaims.ToList();
                if (!Session.Contains(AppCode + "CurrentCommunity"))
                {
                    var communities = (SelectList)ViewData["Communities"];
                    Session.SetItem(AppCode + "CurrentCommunity", communities.First().Value);
                }
                return View(new CurrentClaimListViewModel
                {
                    Claims = compCliamsEnumerated,
                    CommunityId = compCliamsEnumerated.First().CommunityId
                });
            }
        }

        #region Private Helper Functions
        private ActionResult RMWCurrentEmployeeClaim()
        {
            Session.TryGetObject(AppCode + "CurrentEmployeeClaimId", out string claimId);
            using (var context = new WorkersCompContext())
            {
                var claimCurrent = context.WorkersCompClaims.Single(c => c.ClaimId == claimId);
                var employeeCurrent = Context.Employees.Single(e => e.EmployeeId == claimCurrent.EmployeeId);
                var diagnosesCurrent = context.WorkersCompDiagnoses.Where(d => d.ClaimId == claimCurrent.ClaimId).ToList();
                var expensesCurrent = context.WorkersCompExpenses.Where(e => e.ClaimId == claimId).ToList();
                var notesForCurrent = context.WorkersCompClaimNotes.Where(n => n.ClaimId == claimId).ToList();
                Session.SetItem(AppCode + "CurrentEmployeeName", employeeCurrent.LastName + ", " + employeeCurrent.FirstName);
                Session.SetItem(AppCode + "CurrentCommunity", employeeCurrent.CommunityId);
                return View(new RMWorkerCompViewModel
                {
                    CompClaim = claimCurrent,
                    DiagnosesForClaim = diagnosesCurrent,
                    DiagnosisTypes = context.WorkersCompDiagnosisTypes.ToList(),
                    Expenses = expensesCurrent,
                    Notes = notesForCurrent
                });
            }
        }

        private void SetWorkersCompInsuranceDropDown()
        {
            using (var context = new WorkersCompContext())
            {
                ViewData["Insurance"] = new SelectList(context.WorkersCompInsurances.Where(i => i.DataEntryFlg)
                                        .OrderBy(i => i.SortOrder).ThenBy(i => i.InsuranceNm).ToList(), "InsuranceId", "InsuranceNm");
                ViewData["Litigated"] = new SelectList(context.WorkersCompLegalFirms.Where(l => l.DataEntryFlg)
                                        .OrderBy(l => l.SortOrder).ThenBy(l => l.LegalFirmNm).ToList(), "LegalFirmID", "LegalFirmNm");
                ViewData["VOCRehab"] = new SelectList(context.WorkersCompVOCRehabs.Where(v => v.DataEntryFlg)
                                        .OrderBy(v => v.SortOrder).ThenBy(v => v.VOCRehabName).ToList(), "VOCRehabID", "VOCRehabName");
                ViewData["TCM"] = new SelectList(context.WorkersCompTCMs.Where(t => t.DataEntryFlg)
                                        .OrderBy(t => t.SortOrder).ThenBy(t => t.TCMName).ToList(), "TCMId", "TCMName");
            }
        }
        private void SetWorkersCompClaimTypeDropDown()
        {
            using (var context = new WorkersCompContext())
            {
                ViewData["ClaimTypes"] = new SelectList(context.CompClaimTypes.Where(c => c.DataEntryFlg)
                                        .OrderBy(c => c.SortOrder).ThenBy(c => c.ClaimTypeDesc).ToList(), "ClaimTypeId", "ClaimTypeDesc");
            }
        }
        #endregion
        //internal void SetWorkersCompDiagnosisDropDown()
        //{
        //    if (Session[AppCode + "CompDiagnoses"] == null)
        //    {
        //        var compDiagnosisContext = new WorkersCompDiagnosisContext();

        //        Session[AppCode + "CompDiagnoses"] = new SelectList(compDiagnosisContext.WorkersCompDiagnoses.OrderBy(c => c.DiagnosisDesc).ToList(),
        //                                                            "DiagnosisId", "DiagnosisDesc");
        //    }
        //}

        #region Save New/Edit
        [HttpPost]
        public ActionResult SaveClaimDetails(IFormCollection form)
        {
            if (!Session.TryGetObject(AppCode + "CurrentEmployeeClaimId", out string claimId))
            {
                return RedirectToAction("");
            }
            using (var context = new WorkersCompContext())
            {
                var currentClaim = context.WorkersCompClaims.Single(c => c.ClaimId == claimId);
                currentClaim.ClaimTypeId = Int32.Parse(form["CompClaim.ClaimTypeId"]);
                currentClaim.LightDutyBeginDate = (string.IsNullOrEmpty(form["CompClaim.LightDutyBeginDate"])
                                                       ? (DateTime?)null
                                                       : DateTime.Parse(form["CompClaim.LightDutyBeginDate"]));
                currentClaim.LightDutyEndDate = (string.IsNullOrEmpty(form["CompClaim.LightDutyEndDate"])
                                                       ? (DateTime?)null
                                                       : DateTime.Parse(form["CompClaim.LightDutyEndDate"]));
                currentClaim.FullDutyBeginDate = (string.IsNullOrEmpty(form["CompClaim.FullDutyBeginDate"])
                                                       ? (DateTime?)null
                                                       : DateTime.Parse(form["CompClaim.FullDutyBeginDate"]));
                currentClaim.FullDutyEndDate = (string.IsNullOrEmpty(form["CompClaim.FullDutyEndDate"])
                                                       ? (DateTime?)null
                                                       : DateTime.Parse(form["CompClaim.FullDutyEndDate"]));
                currentClaim.InsuranceCarrierId = Int32.Parse(form["Insurance"]);
                currentClaim.LegalFirmFlg = form["CompClaim.LegalFirmFlg"].Contains("true");
                currentClaim.LegalFirmID = (currentClaim.LegalFirmFlg ? Int32.Parse(form["Litigated"]) : (int?)null);
                currentClaim.VOCRehabFlg = form["CompClaim.VOCRehabFlg"].Contains("true");
                currentClaim.VOCRehabID = (currentClaim.VOCRehabFlg ? Int32.Parse(form["VOCRehab"]) : (int?)null);
                currentClaim.TCMFlg = form["CompClaim.TCMFlg"].Contains("true");
                currentClaim.TCMId = (currentClaim.TCMFlg ? Int32.Parse(form["TCM"]) : (int?)null);
                currentClaim.LastUser = User.Identity.Name;
                var newReportedToCarrier = DateTime.Parse(form["CompClaim.ReportedtoCarrierDate"]);
                if (currentClaim.ReportedtoCarrierDate != newReportedToCarrier)
                {
                    currentClaim.ReportedtoCarrierDate = newReportedToCarrier;
                    currentClaim.ReportedToCarrierDateOverrideFlag = true;
                }
                currentClaim.FROIDate = (String.IsNullOrEmpty(form["CompClaim.FROIDate"]) ? (DateTime?)null : DateTime.Parse(form["CompClaim.FROIDate"]));
                currentClaim.PreventableFlg = form["CompClaim.PreventableFlg"].Contains("true");
                currentClaim.PreventableComments = (currentClaim.PreventableFlg
                                                        ? form["CompClaim.PreventableComments"].First()
                                                        : null);
                currentClaim.HighExposureFlg = form["CompClaim.HighExposureFlg"].Contains("true");
                currentClaim.HighExposureComments = (currentClaim.HighExposureFlg
                                                        ? form["CompClaim.HighExposureComments"].First()
                                                        : null);
                context.SaveChanges();
            }
            return RedirectToAction("");
        }

        [HttpPost]
        public ActionResult SaveNotes(string UNotes, string NotesId)
        {
            if (!Session.TryGetObject(AppCode + "CurrentEmployeeClaimId", out string claimId))
            {
                return RedirectToAction("");
            }
            using (var context = new WorkersCompContext())
            {
                var compNotes = (String.IsNullOrEmpty(NotesId)
                                      ? new WorkersCompClaimNotes()
                                      : context.WorkersCompClaimNotes.Find(Int32.Parse(NotesId)));
                compNotes.Notes = UNotes;
                if (String.IsNullOrEmpty(NotesId))
                {
                    compNotes.UserName = User.Identity.Name;
                    compNotes.InsertedDate = DateTime.Now;
                    compNotes.ClaimId = claimId;
                    context.WorkersCompClaimNotes.Add(compNotes);
                }
                context.SaveChanges();
            }
            return RedirectToAction("");
        }
        #endregion

        #region Update Post Backs
        [HttpPost]
        public ActionResult UpdateRange(string occurredRangeFrom, string occurredRangeTo, string returnUrl, int currentCommunity)
        {
            Session.SetItem(AppCode + "CurrentCommunity",currentCommunity);
            return UpdateTableRange(occurredRangeFrom, occurredRangeTo, returnUrl, AppCode);
        }
        #endregion

        #region Ajax Calls
        public JsonResult SetClaimId(string claimId)
        {
            Session.SetItem(AppCode + "CurrentEmployeeClaimId", claimId);
            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion
    }
}
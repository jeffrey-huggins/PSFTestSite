using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Financial.Models;
using AtriumWebApp.Web.Financial.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "ADR")]
    public class AdditionalDevelopmentRequestController : BaseController
    {
        public AdditionalDevelopmentRequestController(IOptions<AppSettingsConfig> config, AdditionalDevelopmentRequestContext context) : base(config, context)
        {

        }

        private const string AppCode = "ADR";

        protected new AdditionalDevelopmentRequestContext Context
        {
            get { return (AdditionalDevelopmentRequestContext)base.Context; }
        }

        public ActionResult Index()
        {
            LogSession(AppCode);
            SetDateRangeErrorValues();
            SetLookbackDays(HttpContext, AppCode);
            SetInitialTableRangeLookback(AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            ViewData["Communities"] = new SelectList(FacilityList[AppCode], "CommunityId", "CommunityShortName");
            GetADRDropDowns();
            var currentCommunity = CurrentFacility[AppCode];
            var fromDate = DateTime.Parse(OccurredRangeFrom[AppCode]);
            var toDate = DateTime.Parse(OccurredRangeTo[AppCode]);
            var filteredADRByDate = Context.ADRs.Where(adr => adr.CommunityId == currentCommunity
                                                           && adr.ADRCMSDate >= fromDate
                                                           && adr.ADRCMSDate <= toDate).ToList();
            bool canDelete = DetermineObjectAccess("0002", currentCommunity, AppCode);
            bool CanRequestNotes = DetermineObjectAccess("0001", currentCommunity, AppCode);
            if (Session.Contains(AppCode + "CurrentADRId"))
            {
                string adrId;
                Session.TryGetObject(AppCode + "CurrentADRId",out adrId);
                return View(new ADRViewModel
                {
                    ADR = Context.ADRs.Single(adr => adr.RequestId == adrId),
                    ADRForCommunity = filteredADRByDate,
                    CanDelete = DetermineObjectAccess("0002", currentCommunity, AppCode),
                    CanRequestNotes = DetermineObjectAccess("0001", currentCommunity, AppCode)

                });
            }
            return View(new ADRViewModel
            {
                ADRForCommunity = filteredADRByDate,
                CanDelete = DetermineObjectAccess("0002", currentCommunity, AppCode),
                CanRequestNotes = DetermineObjectAccess("0001", currentCommunity, AppCode)
            });
        }
        #region Private Helper Functions
        private void GetADRDropDowns()
        {
            ViewData["Payer"] = new SelectList(Context.ADRPayers.ToList(), "ADRPayerId", "ADRPayerName");
        }
        #endregion

        #region Save New/Edit
        public ActionResult SaveADR(IFormCollection form)
        {
            var adr = !String.IsNullOrEmpty(form["ADR.RequestId"])
                          ? Context.ADRs.Find(form["ADR.RequestId"])
                          : new AdditionalDevelopmentRequest();
            adr.FirstName = form["ADR.FirstName"];
            adr.LastName = form["ADR.LastName"];
            adr.MedicareNumber = form["ADR.MedicareNumber"];
            adr.CommunityId = CurrentFacility[AppCode];
            adr.ADRPayerId = Int32.Parse(form["ADR.ADRPayerId"]);
            adr.ARAmount = decimal.Parse(form["ADR.ARAmount"]);
            adr.ADRCMSDate = DateTime.Parse(form["ADR.ADRCMSDate"]);
            adr.ADRReceivedDate = String.IsNullOrEmpty(form["ADR.ADRReceivedDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.ADRReceivedDate"]);
            adr.ADRReturnMailDate = String.IsNullOrEmpty(form["ADR.ADRReturnMailDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.ADRReturnMailDate"]);
            adr.ADRDenialDate = String.IsNullOrEmpty(form["ADR.ADRDenialDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.ADRDenialDate"]);
            adr.ServiceBeginDate = String.IsNullOrEmpty(form["ADR.ServiceBeginDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.ServiceBeginDate"]);
            adr.ServiceEndDate = String.IsNullOrEmpty(form["ADR.ServiceEndDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.ServiceEndDate"]);
            adr.RedeterminationMailDate = String.IsNullOrEmpty(form["ADR.RedeterminationMailDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.RedeterminationMailDate"]);
            adr.RedeterminationDenialDate = String.IsNullOrEmpty(form["ADR.RedeterminationDenialDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.RedeterminationDenialDate"]);
            adr.ReconsiderationMailDate = String.IsNullOrEmpty(form["ADR.ReconsiderationMailDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.ReconsiderationMailDate"]);
            adr.ReconsiderationDenialDate = String.IsNullOrEmpty(form["ADR.ReconsiderationDenialDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.ReconsiderationDenialDate"]);
            adr.ALJMailDate = String.IsNullOrEmpty(form["ADR.ALJMailDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.ALJMailDate"]);
            adr.ALJHearingDate = String.IsNullOrEmpty(form["ADR.ALJHearingDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.ALJHearingDate"]);
            adr.ALJDenialDate = String.IsNullOrEmpty(form["ADR.ALJDenialDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.ALJDenialDate"]);
            adr.DCN = form["ADR.DCN"];
            adr.RevisedDCN = form["ADR.RevisedDCN"];
            adr.RACLetterID = form["ADR.RACLetterID"];
            adr.DemandLetter = form["ADR.DemandLetter"];
            adr.C2CMedicareAppeal = form["ADR.C2CMedicareAppeal"];
            adr.RemitApproveDate = String.IsNullOrEmpty(form["ADR.RemitApproveDate"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["ADR.RemitApproveDate"]);
            adr.ClosedFlg = form["ADR.ClosedFlg"].Contains("true");
            if (String.IsNullOrEmpty(form["ADR.RequestId"]))
            {
                string userName = HttpContext.User.Identity.Name;
                if (userName == null)
                {
                    userName = WindowsIdentity.GetCurrent().Name;
                }

                adr.RequestId = userName + DateTime.Now.ToString("yyyyMMddHHmmss");
                Context.ADRs.Add(adr);
            }
            if (this.IsUserAdmin(this.PrincipalContext, this.UserPrincipal))
            {
                adr.RequestNotes = form["ADR.RequestNotes"];
            }
            Context.SaveChanges();
            Session.SetItem(AppCode + "CurrentADRId", adr.RequestId);
            return RedirectToAction("");
        }
        #endregion

        #region Update Post Backs
        [HttpPost]
        public ActionResult UpdateRange(string occurredRangeFrom, string occurredRangeTo, string returnUrl)
        {
            return UpdateTableRange(occurredRangeFrom, occurredRangeTo, returnUrl, AppCode);
        }
        [HttpPost] //Side Drop Down List update
        public ActionResult SideDDL(string Communities, string returnUrl)
        {
            var facilityId = 0;
            if (!Int32.TryParse(Communities, out facilityId))
            {
                return Redirect(returnUrl);
            }
            CurrentFacility[AppCode] = facilityId;
            var facility = (from fac in Context.Facilities
                            where fac.CommunityId == facilityId
                            select fac).Single();
            CurrentFacilityName[AppCode] = facility.CommunityShortName;
            Session.Remove(AppCode + "CurrentADRId");
            return Redirect(returnUrl);
        }
        #endregion

        #region Ajax Calls
        public void ClearADR()
        {
            Session.Remove(AppCode + "CurrentADRId");
        }

        public void SetADR(string rowId)
        {
            Session.SetItem(AppCode + "CurrentADRId", rowId);
        }

        public void ClosedRequests(bool isChecked)
        {
            Session.SetItem("ClosedRequestsShown",isChecked);
        }

        public JsonResult AddNote(string adrId, string note)
        {
            string userName = HttpContext.User.Identity.Name;
            if (userName == null)
            {
                userName = WindowsIdentity.GetCurrent().Name;
            }
            var finalNote = "[" + userName + " " + DateTime.Now.ToString("G") + "]\n" + note;
            var adr = Context.ADRs.Find(adrId);
            adr.RequestNotes = (adr.RequestNotes != null ? adr.RequestNotes.Insert(adr.RequestNotes.Length, "\n\n" + finalNote) : finalNote);
            try
            {
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                SqlException se;
                do
                {
                    se = ex.InnerException as SqlException;
                    ex = ex.InnerException;
                }
                while (ex != null && se == null);
                if (se != null && se.Number == 2627)
                {
                    return Json(new { Success = false, Reason = 0 });
                }
                if (se != null && se.Number == 8152)
                {
                    return Json(new { Success = false, Reason = 2 });
                }
                return Json(new { Success = false, Reason = 1 });
            }
            return Json(new { Success = true, UpdateNote = adr.RequestNotes });
        }

        public JsonResult DeleteRow(string rowId)
        {
            Context.ADRs.Remove(Context.ADRs.Find(rowId));
            Context.SaveChanges();

            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion
    }
}
using System;
using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Models.Enumerations;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.RiskManagement.Models;
using AtriumWebApp.Web.RiskManagement.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.RiskManagement.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "RMW", Admin = true)]
    public class RMWorkerCompAdminController : BaseAdminController
    {
        private const string AppCode = "RMW";

        public RMWorkerCompAdminController(IOptions<AppSettingsConfig> config, SharedContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            //If user doesn't have access, redirect to the home page
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            SetLookbackDaysAdmin(HttpContext, AppCode);
            //Get information for type tables
            using (var context = new WorkersCompContext())
            {
                var adminViewModel = CreateAdminViewModel(AppCode);
                return View(new RMWAdminViewModel
                {
                    AdminViewModel = adminViewModel,
                    Insurances = context.WorkersCompInsurances.ToList(),
                    LegalFirms = context.WorkersCompLegalFirms.ToList(),
                    VOCRehabs = context.WorkersCompVOCRehabs.ToList(),
                    TCMs = context.WorkersCompTCMs.ToList(),
                    ClaimTypes = context.CompClaimTypes.ToList()
                });
            }
        }

        #region Create New Types
        [HttpPost]
        public ActionResult SaveNewInsurance(WorkersCompInsurance NewInsurance)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }

            using (var context = new WorkersCompContext())
            {
                context.WorkersCompInsurances.Add(NewInsurance);
                context.SaveChanges();
            }
            return Json(new { Success = true, data = NewInsurance });
        }

        [HttpPost]
        public ActionResult SaveNewLegalFirm(WorkersCompLegalFirm NewLegalFirm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }

            using (var context = new WorkersCompContext())
            {
                context.WorkersCompLegalFirms.Add(NewLegalFirm);
                context.SaveChanges();
            }
            return Json(new { Success = true, data = NewLegalFirm });
        }

        [HttpPost]
        public ActionResult SaveNewVOCRehab(WorkersCompVOCRehab NewVOCRehab)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }

            using (var context = new WorkersCompContext())
            {
                context.WorkersCompVOCRehabs.Add(NewVOCRehab);
                context.SaveChanges();
            }
            return Json(new { Success = true, data = NewVOCRehab });

        }

        [HttpPost]
        public ActionResult SaveNewTCM(WorkersCompTCM NewTCM)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }

            using (var context = new WorkersCompContext())
            {
                context.WorkersCompTCMs.Add(NewTCM);
                context.SaveChanges();
            }
            return Json(new { Success = true, data = NewTCM });

        }

        [HttpPost]
        public ActionResult SaveNewType(WorkersCompClaimType NewType)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }

            using (var context = new WorkersCompContext())
            {
                context.CompClaimTypes.Add(NewType);
                context.SaveChanges();
            }
            return Json(new { Success = true, data = NewType });
        }

        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            using (var appContext = new SharedContext())
            {
                var appInfo = (from app in appContext.Applications
                               where app.ApplicationCode == AppCode
                               select app).Single();
                var lbDays = Int32.Parse(lookbackDays);
                appInfo.LookbackDays = lbDays;
                appContext.SaveChanges();
                Session.SetItem(AppCode + "LookbackDays", lbDays);
            }
            return RedirectToAction("");
        }
        #endregion

        #region Ajax Calls
        public JsonResult DeleteRowType(int rowId)
        {
            using (var context = new WorkersCompContext())
            {
                context.CompClaimTypes.Remove(context.CompClaimTypes.Find(rowId));
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult EditRowType(int rowId, string type, int order)
        {
            using (var context = new WorkersCompContext())
            {
                var typeRecord = context.CompClaimTypes.Find(rowId);
                typeRecord.ClaimTypeDesc = type;
                typeRecord.SortOrder = order;
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult DeleteRowInsurance(int rowId)
        {
            using (var context = new WorkersCompContext())
            {
                context.WorkersCompInsurances.Remove(context.WorkersCompInsurances.Find(rowId));
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult EditRowInsurance(int rowId, string insurance, int order)
        {
            using (var context = new WorkersCompContext())
            {
                var insuranceRecord = context.WorkersCompInsurances.Find(rowId);
                insuranceRecord.InsuranceNm = insurance;
                insuranceRecord.SortOrder = order;
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult DeleteRowLawFirm(int rowId)
        {
            using (var context = new WorkersCompContext())
            {
                context.WorkersCompLegalFirms.Remove(context.WorkersCompLegalFirms.Find(rowId));
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult EditRowLawFirm(int rowId, string lawFirm, int order)
        {
            using (var context = new WorkersCompContext())
            {
                var legalFirmRecord = context.WorkersCompLegalFirms.Find(rowId);
                legalFirmRecord.LegalFirmNm = lawFirm;
                legalFirmRecord.SortOrder = order;
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult DeleteRowVOC(int rowId)
        {
            using (var context = new WorkersCompContext())
            {
                context.WorkersCompVOCRehabs.Remove(context.WorkersCompVOCRehabs.Find(rowId));
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult EditRowVOC(int rowId, string voc, int order)
        {
            using (var context = new WorkersCompContext())
            {
                var vocRehab = context.WorkersCompVOCRehabs.Find(rowId);
                vocRehab.VOCRehabName = voc;
                vocRehab.SortOrder = order;
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult DeleteRowTCM(int rowId)
        {
            using (var context = new WorkersCompContext())
            {
                context.WorkersCompTCMs.Remove(context.WorkersCompTCMs.Find(rowId));
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult EditRowTCM(int rowId, string tcm, int order)
        {
            using (var context = new WorkersCompContext())
            {
                var tcmRecord = context.WorkersCompTCMs.Find(rowId);
                tcmRecord.TCMName = tcm;
                tcmRecord.SortOrder = order;
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult ChangeDataFlg(string description, bool dFlag, RMWCode rmwCode)
        {
            using (var context = new WorkersCompContext())
            {
                switch (rmwCode)
                {
                    case RMWCode.Insurance:
                        var insurance = context.WorkersCompInsurances.Single(i => i.InsuranceNm == description);
                        insurance.DataEntryFlg = dFlag;
                        break;
                    case RMWCode.LawFirm:
                        var lawFirm = context.WorkersCompLegalFirms.Single(l => l.LegalFirmNm == description);
                        lawFirm.DataEntryFlg = dFlag;
                        break;
                    case RMWCode.TCM:
                        var tcm = context.WorkersCompTCMs.Single(t => t.TCMName == description);
                        tcm.DataEntryFlg = dFlag;
                        break;
                    case RMWCode.VOC:
                        var voc = context.WorkersCompVOCRehabs.Single(v => v.VOCRehabName == description);
                        voc.DataEntryFlg = dFlag;
                        break;
                    case RMWCode.Type:
                        var type = context.CompClaimTypes.Single(t => t.ClaimTypeDesc == description);
                        type.DataEntryFlg = dFlag;
                        break;
                }
                context.SaveChanges();
            }
            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion
    }
}
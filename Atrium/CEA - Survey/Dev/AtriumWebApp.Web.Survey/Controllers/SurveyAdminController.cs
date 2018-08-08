using System;
using System.Data.Entity;
using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Survey.Models;
using AtriumWebApp.Web.Survey.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Survey.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "CSU", Admin = true)]
    public class SurveyAdminController : BaseAdminController
    {
        private const string AppCode = "CSU";

        public SurveyAdminController(IOptions<AppSettingsConfig> config, SurveyContext context) : base(config, context)
        {
        }

        protected new SurveyContext Context
        {
            get { return (SurveyContext)base.Context; }
        }


        public ActionResult Index()
        {
            //If user doesn't have access, redirect to the home page
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            var adminViewModel = CreateAdminViewModel(AppCode);
            //Information for each Type Table
            return View(new SurveyAdminViewModel
            {
                SurveyTypes = Context.CommunitySurveyTypes.ToList(),
                SASs = Context.ScopeAndSeverities.ToList(),
                FederalDeficiencies = Context.FederalDeficiencies.Include(e => e.AtriumPayerGroup).ToList(),
                StateDeficiencies = Context.StateDeficiencies.ToList(),
                SafetyDeficiencies = Context.SafetyDeficiencies.ToList(),
                StateCodes = new SelectList(Context.States.ToList(), "StateCd", "StateCd"),
                SurveyPayerGroups = new SelectList(Context.AtriumPayerGroups.Where(e => e.IsCommunitySurveyEligible).ToList(), "AtriumPayerGroupCode", "AtriumPayerGroupName"),
                AdminViewModel = adminViewModel
            });
        }

        #region Json

        public JsonResult ChangeComplaintFlgSurvey(int surveyTypeId, bool complaintFlag)
        {
            var masterCommSurveyTypeRow = Context.CommunitySurveyTypes.Single(t => t.SurveyTypeId == surveyTypeId);
            masterCommSurveyTypeRow.ComplaintFlg = complaintFlag;
            try
            {
                Context.SaveChanges();
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        #endregion

        #region Create New Types

        public ActionResult NewSurveyType(CommunitySurveyType surveyType)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.CommunitySurveyTypes.Add(surveyType);
            Context.SaveChanges();
            return Json(new { success = true,data = surveyType });
        }

        public ActionResult NewSasType(ScopeAndSeverity sas)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.ScopeAndSeverities.Add(sas);
            Context.SaveChanges();
            return Json(new { success = true, data = sas });
            //return RedirectToAction("");
        }

        public ActionResult NewFedDef(FederalDeficiencyViewModel FedDef)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            var fedDef = FedDef.Deficiency;
            Context.FederalDeficiencies.Add(fedDef);
            Context.SaveChanges();
            return Json(new { success = true, data = fedDef });
        }

        public ActionResult NewStateDef(StateDeficiencyViewModel stateDef)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.StateDeficiencies.Add(stateDef.Deficiency);
            Context.SaveChanges();
            return Json(new { success = true, data = stateDef.Deficiency });
        }

        public ActionResult NewSafetyDef(SafetyDeficiency safetyDef)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.SafetyDeficiencies.Add(safetyDef);
            Context.SaveChanges();
            return Json(new { success = true, data = safetyDef });
        }
        #endregion

        #region Edit Type Names
        public JsonResult EditRowSurveyType(int rowId, string description, bool complaintFlag)
        {
            var surveyType = Context.CommunitySurveyTypes.Find(rowId);
            surveyType.SurveyTypeDesc = description;
            surveyType.ComplaintFlg = complaintFlag;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult EditRowSAS(int rowId, string description)
        {
            var sas = Context.ScopeAndSeverities.Find(rowId);
            sas.SASCode = description;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult EditRowFedDef(int rowId, string def, string payerGroup, string description)
        {
            var fedDef = Context.FederalDeficiencies.Find(rowId);
            fedDef.TagCode = def;
            fedDef.AtriumPayerGroupCode = payerGroup;
            fedDef.Description = description;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult EditRowStateDef(int rowId, string stateCd, string def, string description)
        {
            var stateDef = Context.StateDeficiencies.Find(rowId);
            stateDef.StateCode = stateCd;
            stateDef.TagCode = def;
            stateDef.Description = description;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult EditRowSafetyDef(int rowId, string def, string description)
        {
            var safetyDef = Context.SafetyDeficiencies.Find(rowId);
            safetyDef.TagCode = def;
            safetyDef.Description = description;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion

        #region Delete Types
        public JsonResult DeleteRowSurveyType(int rowId)
        {
            try
            {
                Context.CommunitySurveyTypes.Remove(Context.CommunitySurveyTypes.Find(rowId));
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                return Json(new { Success = false, e.Message });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult DeleteRowSAS(int rowId)
        {
            try
            {
                Context.ScopeAndSeverities.Remove(Context.ScopeAndSeverities.Find(rowId));
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                return Json(new { Success = false, e.Message });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult DeleteRowFedDef(int rowId)
        {
            try
            {
                Context.FederalDeficiencies.Remove(Context.FederalDeficiencies.Find(rowId));
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                return Json(new { Success = false, e.Message });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult DeleteRowStateDef(int rowId)
        {
            try
            {
                Context.StateDeficiencies.Remove(Context.StateDeficiencies.Find(rowId));
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                return Json(new { Success = false, e.Message });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult DeleteRowSafetyDef(int rowId)
        {
            var safetyDef = Context.SafetyDeficiencies.Find(rowId);
            try
            {
                Context.SafetyDeficiencies.Remove(Context.SafetyDeficiencies.Find(rowId));
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                return Json(new { Success = false, e.Message });
            }
            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion
    }
}
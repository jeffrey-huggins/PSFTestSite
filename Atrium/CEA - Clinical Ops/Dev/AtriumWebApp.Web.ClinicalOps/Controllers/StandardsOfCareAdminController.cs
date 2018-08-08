using System;
using System.Linq;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "SOC")]
    public class StandardsOfCareAdminController : BaseAdminController
    {
        private const string AppCode = "SOC";

        public StandardsOfCareAdminController(IOptions<AppSettingsConfig> config, SOCContext context) : base(config, context)
        {
        }

        protected new SOCContext Context
        {
            get { return (SOCContext)base.Context; }
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
            var adminViewModel = CreateAdminViewModel(AppCode);
            return View(new StandardsOfCareAdminViewModel
            {
                PayerGroups = Context.AtriumPayerGroups.ToList(),
                CommunityPayerGroupInfo = Context.PayerGroupInfos.Where(a => a.ApplicationId == adminViewModel.AppId).ToList(),
                Measures = Context.MasterSOCMeasure.OrderBy(d => d.SortOrder).ThenBy(d => d.SOCMeasureName).ToList(),
                AdminViewModel = adminViewModel,
                SOCAintiPsychoticDiagnosis = Context.AntiPsychoticDiagnoses.ToList(),
                CatheterTypes = Context.CatheterTypes.ToList(),
                SOCFallLocation = Context.FallLocations.ToList(),
                SOCFallInjuryType = Context.FallInjuryTypes.ToList(),
                SOCFallTreatment = Context.FallTreatmentTypes.ToList(),
                SOCFallIntervention = Context.FallInterventionTypes.ToList(),
                SOCFallType = Context.FallTypes.ToList(),
                Restraints = Context.Restraints.ToList(),
                PressureWoundStages = Context.PressureWoundStages.OrderBy(p => p.SortOrder).ThenBy(p => p.PressureWoundStageName).ToList(),
                CompositeWoundDescribe = Context.CompositeWoundDescribes.OrderBy(c => c.SortOrder).ThenBy(c => c.CompositeWoundDescribeName).ToList(),
                Medications = Context.AntiPsychoticMedications.ToList()
              
            });
        }

        #region Create New Types
        [HttpPost]
        public ActionResult NewMeasure(Measure newMeasure)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.MasterSOCMeasure.Add(newMeasure);
            Context.SaveChanges();
            return Json(new { Success = true, data = newMeasure });
        }

        public ActionResult NewCatheter(SOCCatheterType newCatheter)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.CatheterTypes.Add(newCatheter);
            Context.SaveChanges();
            return Json(new { Success = true, data = newCatheter });
        }

        public ActionResult NewAnti(SOCAntiPsychoticDiagnosis newAntipsychotic)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.AntiPsychoticDiagnoses.Add(newAntipsychotic);
            Context.SaveChanges();
            return Json(new { Success = true, data = newAntipsychotic });
        }

        public ActionResult NewLocation(SOCFallLocation newFallLocation)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.FallLocations.Add(newFallLocation);
            Context.SaveChanges();
            return Json(new { Success = true, data = newFallLocation });
        }

        public ActionResult NewInjury(SOCFallInjuryType newFallInjury)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.FallInjuryTypes.Add(newFallInjury);
            Context.SaveChanges();
            return Json(new { Success = true, data = newFallInjury });
        }

        public ActionResult NewTreatment(SOCFallTreatment newFallTreatment)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.FallTreatmentTypes.Add(newFallTreatment);
            Context.SaveChanges();
            return Json(new { Success = true, data = newFallTreatment });
        }

        public ActionResult NewIntervention(SOCFallIntervention newFallIntervention)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.FallInterventionTypes.Add(newFallIntervention);
            Context.SaveChanges();
            return Json(new { Success = true, data = newFallIntervention });
        }

        public ActionResult NewType(SOCFallType newFallType)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.FallTypes.Add(newFallType);
            Context.SaveChanges();
            return Json(new { Success = true, data = newFallType });
        }

        public ActionResult NewStage(PressureWoundStage newPressureWound)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.PressureWoundStages.Add(newPressureWound);
            Context.SaveChanges();
            return Json(new { Success = true, data = newPressureWound });
        }

        public ActionResult NewDescribe(CompositeWoundDescribe newCompositeWound)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.CompositeWoundDescribes.Add(newCompositeWound);
            Context.SaveChanges();
            return Json(new { Success = true, data = newCompositeWound });
        }

        public ActionResult NewRestraint(SOCRestraint newRestraint)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.Restraints.Add(newRestraint);
            Context.SaveChanges();
            return Json(new { Success = true, data = newRestraint });
        }

        public ActionResult NewAntiMedication(SOCAntiPsychoticMedication newMedication)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.AntiPsychoticMedications.Add(newMedication);
            Context.SaveChanges();
            return Json(new { Success = true, data = newMedication });
         
        }

        #endregion

        #region Other Post Backs
        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, AppCode);
            return RedirectToAction("");
        }
        #endregion
    }
}
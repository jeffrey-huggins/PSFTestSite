using System;
using System.Linq;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "IFC")]
    public class InfectionControlAdminController : BaseAdminController
    {
        private const string AppCode = "IFC";

        public InfectionControlAdminController(IOptions<AppSettingsConfig> config, InfectionControlContext context) : base(config, context)
        {
        }

        protected new InfectionControlContext Context
        {
            get { return (InfectionControlContext)base.Context; }
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

            return View(new InfectionControlAdminViewModel
            {
                PayerGroups = Context.AtriumPayerGroups.ToList(),
                CommunityPayerGroupInfo = Context.PayerGroupInfos.Where(a => a.ApplicationId == adminViewModel.AppId).ToList(),
                Limits = Context.PatientIFCLimits.First(),
                Sites = Context.Sites.ToList(),
                Organisms = Context.Organisms.Where(o => o.PatientIFCOrganismId > -1).ToList(),
                Symptoms = Context.Symptoms.ToList(),
                Diagnoses = Context.Diagnoses.ToList(),
                Precautions = Context.Precautions.ToList(),
                Antibiotics = Context.Antibiotics.ToList(),
                AdminViewModel = adminViewModel
            });
        }

        #region Create New Types
        [HttpPost]
        public ActionResult NewSite(PatientIFCSite newSite)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.Sites.Add(newSite);
            Context.SaveChanges();

            return Json(new { Success = true, data = newSite });
        }

        [HttpPost]
        public ActionResult NewDiagnosis(PatientIFCDiagnosis newDiagnosis)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.Diagnoses.Add(newDiagnosis);
            Context.SaveChanges();

            return Json(new { Success = true, data = newDiagnosis });
        }

        [HttpPost]
        public ActionResult NewPrecaution(PatientIFCTypeOfPrecaution newPrecaution)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.Precautions.Add(newPrecaution);
            Context.SaveChanges();

            return Json(new { Success = true, data = newPrecaution });
        }

        [HttpPost]
        public ActionResult NewSymptom(PatientIFCSymptom newSymptom)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.Symptoms.Add(newSymptom);
            Context.SaveChanges();

            return Json(new { Success = true, data = newSymptom });
        }

        [HttpPost]
        public ActionResult NewOrganism(PatientIFCOrganism newOrganism)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.Organisms.Add(newOrganism);
            Context.SaveChanges();
            return Json(new { Success = true, data = newOrganism });
        }

        [HttpPost]
        public ActionResult NewAntibiotic(PatientIFCAntibiotic newAntibiotic)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.Antibiotics.Add(newAntibiotic);
            Context.SaveChanges();
            return Json(new { Success = true, data = newAntibiotic });
        }
        #endregion

        #region Other Post Backs
        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, "IFC");
            return RedirectToAction("");
        }
        #endregion
    }
}
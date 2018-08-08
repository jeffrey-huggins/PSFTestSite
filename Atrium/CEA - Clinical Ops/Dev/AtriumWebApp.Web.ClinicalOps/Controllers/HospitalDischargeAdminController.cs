using System.Data.Entity;
using System.Linq;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "HOD")]
    public class HospitalDischargeAdminController : BaseAdminController
    {
        private const string AppCode = "HOD";

        public HospitalDischargeAdminController(IOptions<AppSettingsConfig> config, HospitalDischargeContext context) : base(config, context)
        {
        }

        protected new HospitalDischargeContext Context
        {
            get { return (HospitalDischargeContext)base.Context; }
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
            AdminViewModel adminViewModel = CreateAdminViewModel(AppCode);
            return View(new HospitalDischargeAdminViewModel
            {
                PayerGroups = Context.AtriumPayerGroups.ToList(),
                CommunityPayerGroupInfo = Context.PayerGroupInfos.Where(a => a.ApplicationId == adminViewModel.AppId).ToList(),
                DischargeReasons = Context.DischargeReasons.OrderBy(d => d.SortOrder).ThenBy(d => d.DischargeReasonDesc).ToList(),
                DidNotReturnReasons = Context.DidNotReturnReasons.OrderBy(d => d.SortOrder).ThenBy(d => d.DidNotReturnReasonDesc).ToList(),
                Hospitals = Context.Hospitals.OrderBy(d => d.SortOrder).ThenBy(d => d.Name).ToList(),
                AdminViewModel = adminViewModel
            });
        }

        public ActionResult GetHospitalAssociations(int id)
        {
            var community = Context.Facilities.SingleOrDefault(c => c.CommunityId == id);
            if (community == null)
            {
                return NotFound("Could not find community with specified Id.");
            }
            var hospitals = Context.Hospitals.Include(h => h.Communities).OrderBy(d => d.SortOrder).ThenBy(d => d.Name);
            var viewModel = new HospitalAssociationsViewModel
            {
                CommunityId = id,
                CommunityName = community.CommunityShortName,
                Associations = hospitals.Select(h => new HospitalAssociationViewModel
                {
                    HospitalId = h.Id,
                    HospitalName = h.Name,
                    IsAssociated = h.Communities.Select(c => c.CommunityId).Contains(id)
                })
            };
            return PartialView(viewModel);
        }

        #region Create New Types
        [HttpPost]
        public ActionResult NewDischargeReason(DischargeReason newDischarge)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.DischargeReasons.Add(newDischarge);
            Context.SaveChanges();
            return Json(new { Success = true, data = newDischarge });
        }

        [HttpPost]
        public ActionResult NewDNRR(DidNotReturnReason NewDNRR)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }

            Context.DidNotReturnReasons.Add(NewDNRR);
            Context.SaveChanges();
            return Json(new { Success = true, data = NewDNRR });
        }

        [HttpPost]
        public ActionResult NewHospital(Hospital NewHospital)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.Hospitals.Add(NewHospital);
            Context.SaveChanges();
            return Json(new { Success = true, data = NewHospital });
        }
        #endregion

        #region Other Post Backs
        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, "HOD");
            return RedirectToAction("");
        }

        [HttpPost]
        public JsonResult ChangeHospitalAssociation(int hospitalId, int communityId, bool isAssociated)
        {
            try
            {
                var hospital = Context.Hospitals.Include(h => h.Communities).Single(h => h.Id == hospitalId);
                var community = Context.Facilities.Single(f => f.CommunityId == communityId);
                if (isAssociated)
                {
                    hospital.Communities.Add(community);
                }
                else
                {
                    hospital.Communities.Remove(community);
                }
                Context.SaveChanges();
                return Json(new SaveResultViewModel { Success = true });
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
        }
        #endregion
    }
}
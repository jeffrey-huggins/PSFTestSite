using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

using AtriumWebApp.Web.HR.Models;

using AtriumWebApp.Models.ViewModel;

using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using AtriumWebApp.Web.HR.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.HR.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "STNA",Admin=true)]
    public class STNAAdminController : BaseAdminController
    {
        protected const string AppCode = "STNA";

        public STNAAdminController(IOptions<AppSettingsConfig> config, STNATrainingContext context) : base(config, context)
        {
        }

        //private STNATrainingContext Context = new STNATrainingContext();
        protected new STNATrainingContext Context
        {
            get { return (STNATrainingContext)base.Context; }
        }
        
        //
        // GET: /STNAAdmin/


        public ActionResult Index()
        {
            //If user doesn't have access, redirect to the home page
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            SelectList stateCodes = new SelectList(Context.States.OrderBy(a => a.StateCd).ToList(), "StateCd", "StateCd");
            
            ViewBag.stateCodes = stateCodes;
            SetLookbackDaysAdmin(HttpContext, AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            var adminViewModel = CreateAdminViewModel(AppCode);
            return View(new STNATrainingAdminViewModel
            {
                TrainingFacilities = Context.STNATrainingFacilities.ToList(),
                //TrainingActionItems = Context.STNATrainingActionItems.ToList(),
                //PayerGroups = Context.AtriumPayerGroups.ToList(),
                //ApplicationCommunityInfo = Context.ApplicationCommunityInfos.Where(a => a.ApplicationId == adminViewModel.AppId).ToList(),
                //Measures = Context.MasterSOCMeasure.OrderBy(d => d.SortOrder).ThenBy(d => d.SOCMeasureName).ToList(),
                AdminViewModel = adminViewModel
            });
        }

        //
        // GET: /STNAAdmin/Details/5

        public ActionResult Details(int id = 0)
        {
            STNATrainingFacility stnatrainingfacility = Context.STNATrainingFacilities.Find(id);
            if (stnatrainingfacility == null)
            {
                return NotFound();
            }
            return View(stnatrainingfacility);
        }

        //
        // GET: /STNAAdmin/Create

        public ActionResult Create()
        {
            SelectList stateCodes = new SelectList(Context.States.OrderBy(a => a.StateCd).ToList(), "StateCd", "StateCd");

            ViewBag.stateCodes = stateCodes;
            return PartialView();
        }
        
        //
        // POST: /STNAAdmin/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(STNATrainingFacility stnatrainingfacility)
        {
            SelectList stateCodes = new SelectList(Context.States.OrderBy(a => a.StateCd).ToList(), "StateCd", "StateCd");

            ViewBag.stateCodes = stateCodes;
            if (ModelState.IsValid)
            {
                Context.STNATrainingFacilities.Add(stnatrainingfacility);
                Context.SaveChanges();
                return Json(new { Success = true, data = stnatrainingfacility });
            }

            return PartialView(stnatrainingfacility);
        }

        //
        // GET: /STNAAdmin/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SelectList stateCodes = new SelectList(Context.States.OrderBy(a => a.StateCd).ToList(), "StateCd", "StateCd");

            ViewBag.stateCodes = stateCodes;
            STNATrainingFacility stnatrainingfacility = Context.STNATrainingFacilities.Find(id);
            if (stnatrainingfacility == null)
            {
                return NotFound();
            }
            return PartialView(stnatrainingfacility);
        }

        //
        // POST: /STNAAdmin/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(STNATrainingFacility stnatrainingfacility)
        {
            SelectList stateCodes = new SelectList(Context.States.OrderBy(a => a.StateCd).ToList(), "StateCd", "StateCd");

            ViewBag.stateCodes = stateCodes;
            if (ModelState.IsValid)
            {
                Context.Entry(stnatrainingfacility).State = EntityState.Modified;
                Context.SaveChanges();
                return Json(new { Success = true, data = stnatrainingfacility });
            }
            return PartialView(stnatrainingfacility);
        }

        //
        // GET: /STNAAdmin/Delete/5

        public ActionResult Delete(int id = 0)
        {
            STNATrainingFacility stnatrainingfacility = Context.STNATrainingFacilities.Find(id);
            if (stnatrainingfacility == null)
            {
                return NotFound();
            }
            return View(stnatrainingfacility);
        }

        //
        // POST: /STNAAdmin/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            STNATrainingFacility stnatrainingfacility = Context.STNATrainingFacilities.Find(id);
            Context.STNATrainingFacilities.Remove(stnatrainingfacility);
            Context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }



        #region Create New Types
        //[HttpPost]
        //public ActionResult NewTrainingActionItem(string NewActionItem)
        //{
        //    if (Context.STNATrainingActionItems.Count(m => m.ActionItemDesc.ToLower().Equals(NewActionItem.ToLower())) != 0 || NewActionItem.Equals(""))
        //    {
        //        return RedirectToAction("");
        //    }
        //    var newActionItem = new STNATrainingActionItem()
        //    {
        //        ActionItemDesc = NewActionItem,
        //        InsertedDate = DateTime.Now
        //        //,
        //        //ReportFlg = true,
        //        //ResolveRequiredFlg = false,
        //        //DataEntryFlg = true,
        //        //OccuranceCalcFlg = true,
        //        //PatientCalcFlg = false
        //    };
        //    Context.STNATrainingActionItems.Add(newActionItem);
        //    Context.SaveChanges();
        //    return RedirectToAction("");
        //}
        #endregion

        #region Other Post Backs
        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, AppCode);
            return RedirectToAction("");
        }
        #endregion


        #region Community Association

        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 600)]
        public ActionResult GetCommunityAssociations(int id)
        {
            var facility = Context.STNATrainingFacilities.Include(f => f.Communities).SingleOrDefault(c => c.STNATrainingFacilityId == id);
            if (facility == null)
            {
                return NotFound("Could not find facility with specified Id.");
            }

            var communities = FacilityList[AppCode];//Context.Facilities.Where(c => c.IsCommunityFlg == true).OrderBy(f => f.CommunityShortName).ToList();

            var viewModel = new STNAFacilityAssociationsViewModel
            {

                STNATrainingFacilityId = id,
                TrainingFacilityName = facility.TrainingFacilityName,
                Associations = communities.Where(c => c.IsCommunityFlg == true).Select(c => new STNAFacilityAssociationViewModel
                {
                    CommunityId = c.CommunityId,
                    CommunityName = c.CommunityShortName,
                    IsAssociated = facility.Communities.Any(cc => cc.CommunityId == c.CommunityId), //facility.Communities.Contains(c),
                    CanDisassociate = CanDisassociate(id, c.CommunityId)
                })
            };

            return PartialView("GetCommunityAssociations", viewModel);

        }



        [HttpPost]
        public JsonResult ChangeCommunityAssociation(int contractorId, int communityId, bool isAssociated)
        {
            try
            {
                var contractor = Context.STNATrainingFacilities.Include(h => h.Communities).Single(c => c.STNATrainingFacilityId == contractorId);
                var community = Context.Facilities.Single(f => f.CommunityId == communityId);
                if (isAssociated)
                {
                    contractor.Communities.Add(community);
                }
                else if (CanDisassociate(contractorId, communityId))
                {
                    contractor.Communities.Remove(community);
                }
                else
                {
                    return Json(new { Success = false, Message = "Contractor cannot be disassociated from this community." });
                }
                Context.SaveChanges();
                return Json(new SaveResultViewModel { Success = true });
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
        }

        private bool CanDisassociate(int contractorId, int communityId)
        {
            return true;//return (Context.ContractorPayrollTransfers.FirstOrDefault(pt => pt.CommunityId == communityId && pt.STNATrainingFacilityId == contractorId) == null);
        }



        #endregion

    }
}
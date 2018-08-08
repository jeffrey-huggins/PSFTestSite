using System.Data.Entity;
using System.Linq;
using AtriumWebApp.Web.PayrollTransfer.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.PayrollTransfer.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Base.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AtriumWebApp.Web.PayrollTransfer.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "PRTR", Admin = true)]
    public class PayrollTransferAdminController : BaseAdminController
    {
        //private PayrollTransferAdminContext Context = new PayrollTransferAdminContext();
        private const string AppCode = "PRTR";

        public PayrollTransferAdminController(IOptions<AppSettingsConfig> config, PayrollTransferContext context) : base(config, context)
        {
        }

        protected new PayrollTransferContext Context
        {
            get { return (PayrollTransferContext)base.Context; }
        }
        //
        // GET: /PayrollTransferAdmin/

        public ActionResult Index()
        {


            //If user doesn't have access, redirect to the home page
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            SetLookbackDaysAdmin(HttpContext, AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            AdminViewModel adminViewModel = CreateAdminViewModel(AppCode);

            return View(new PayrollTransferAdminViewModel
            {
                Contractors = Context.PTContractors.Include(a => a.Communities).ToList(),
                AdminViewModel = adminViewModel
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PTContractor ptcontractor)
        {
            if (ModelState.IsValid)
            {
                Context.PTContractors.Add(ptcontractor);
                Context.SaveChanges();
                return Json(new { Success = true, data = ptcontractor });
            }

            return RedirectToAction("");
        }

        public ActionResult EditContractor(int id, string firstName, string lastName, string vendorNbr)
        {
            var contractor = Context.PTContractors.Find(id);
            contractor.FirstName = firstName;
            contractor.LastName = lastName;
            contractor.VendorNbr = vendorNbr;

            Context.SaveChanges();
            return Json(new { Success = true });
        }

        public ActionResult DeleteContractor(int id)
        {
            var contractor = Context.PTContractors.Include(a => a.Communities).FirstOrDefault(a => a.PTContractorId == id);
            if(contractor == null)
            {
                return NotFound();
            }
            if(contractor.Communities.Count > 0)
            {
                return Json(new { Success = false, Message = "Contractor still has community associations, remove associations and try again." });
            }
            Context.PTContractors.Remove(contractor);
            Context.SaveChanges();
            return Json(new { Success = true });
        }

        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 600)]
        public ActionResult GetCommunityAssociations(int id)
        {
            var contractor = Context.PTContractors.Include(f => f.Communities).SingleOrDefault(c => c.PTContractorId == id);
            if (contractor == null)
            {
                return NotFound("Could not find contractor with specified Id.");
            }

            var communities = FacilityList[AppCode];//Context.Facilities.Where(c => c.IsCommunityFlg == true).OrderBy(f => f.CommunityShortName).ToList();

            var viewModel = new ContractorAssociationsViewModel
            {
                ContractorId = id,
                FirstName = contractor.FirstName,
                LastName = contractor.LastName,
                Associations = communities.Where(c => c.IsCommunityFlg == true).Select(c => new ContractorAssociationViewModel
                {
                    CommunityId = c.CommunityId,
                    CommunityName = c.CommunityShortName,
                    IsAssociated = contractor.Communities.Any(cc => cc.CommunityId == c.CommunityId), //contractor.Communities.Contains(c),
                    CanDisassociate = CanDisassociate(id, c.CommunityId)
                })
            };

            return PartialView("ContractorAssociations", viewModel);

        }



        [HttpPost]
        public JsonResult ChangeCommunityAssociation(int contractorId, int communityId, bool isAssociated)
        {
            try
            {
                var contractor = Context.PTContractors.Include(h => h.Communities).Single(c => c.PTContractorId == contractorId);
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

        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            var appInfo = (from app in Context.Applications
                           where app.ApplicationCode == AppCode
                           select app).Single();
            var lbDays = Int32.Parse(lookbackDays);
            appInfo.LookbackDays = lbDays;
            Context.SaveChanges();
            Session.SetItem(AppCode + "LookbackDays", lbDays);

            return RedirectToAction("");
        }

        private bool CanDisassociate(int contractorId, int communityId)
        {
            return (Context.ContractorPayrollTransfers.FirstOrDefault(pt => pt.CommunityId == communityId && pt.PTContractorId == contractorId) == null);
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }
    }
}
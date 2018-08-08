using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.RiskManagement.Models;
using AtriumWebApp.Web.RiskManagement.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.RiskManagement.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "RMU")]
    public class RMUnemploymentController : BaseRiskManagementController
    {
        private const string AppCode = "RMU";

        protected new UnemploymentContext Context
        {
            get { return (UnemploymentContext)base.Context; }
        }

        public RMUnemploymentController(IOptions<AppSettingsConfig> config, UnemploymentContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            //Record user access
            LogSession(AppCode);
            //Set Census Date Information and Manipulate when changed
            SetLookbackDays(HttpContext, AppCode);
            //Set initial date range values
            SetInitialTableRange(AppCode);

            if (!Session.TryGetObject("EmployeeSideBar", out EmployeeSidebarViewModel sideBar))
            {
                sideBar = SideBarService.InitEmployeeSideBar(this, AppCode, Context, true);
            }

            return View(sideBar);
        }

        #region Save New/Edit
        #region Claims
        public IActionResult CreateOrEditClaim(int employeeId, string claimId)
        {
            ViewBag.UnemploymentCodes = Context.UnemploymentClaimReasons.ToList();
            var claim = new UnemploymentClaim();
            if (string.IsNullOrEmpty(claimId))
            {
                claim.EmployeeId = employeeId;
            }
            else
            {
                claim = Context.UnemploymentClaims.Find(claimId);
            }
            return EditorFor(claim);
        }

        public IActionResult ClaimList(int employeeId)
        {
            return PartialView(Context.UnemploymentClaims.Where(a => a.EmployeeId == employeeId).ToList());
        }

        public IActionResult SaveClaim(UnemploymentClaim vm)
        {
            if (ModelState.IsValid) {
                vm.LastUser = User.Identity.Name;
                if (string.IsNullOrEmpty(vm.ClaimId))
                {
                    vm.ClaimId = User.Identity.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                    Context.UnemploymentClaims.Add(vm);
                }
                else
                {
                    var currentClaim = Context.UnemploymentClaims.Find(vm.ClaimId);
                    Context.Entry(currentClaim).CurrentValues.SetValues(vm);
                }
                Context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, data = "Please make sure all fields are correctly filled out." });
            }
        }

        public IActionResult DeleteClaim(string claimId)
        {
            var claim = Context.UnemploymentClaims.Find(claimId);
            if(claim == null)
            {
                return Json(new { success = false, data = "No claim exists with that ID." });
            }
            else
            {
                Context.UnemploymentClaims.Remove(claim);
                Context.SaveChanges();
                return Json(new { success = true });
            }
        }
        #endregion
        #region Payments
        public IActionResult CreateOrEditPayment(string claimId, int? paymentId)
        {
            var claim = Context.UnemploymentClaims.Include("Employee.Community.State")
                .Include("Payments")
                .FirstOrDefault(a => a.ClaimId == claimId);
            var payPeriodDesc = Context.PaymentPeriods.First(a => a.PaymentPeriodCd == claim.Employee.Community.State.PaymentPeriodCd).PaymentPeriodDesc;
            ViewBag.Cycle = payPeriodDesc;
            
            if (paymentId.HasValue)
            {
                return EditorFor(Context.UnemploymentBenefits.Include("Claim.Payments").FirstOrDefault(a => a.ClaimId == claimId && a.BenefitKey == paymentId.Value));
            }
            else
            {


                var benefit = new UnemploymentBenefit()
                {
                    Claim = claim,
                    ClaimId = claimId,
                    BenefitKey = -1
                };
                if (claim.Payments.Any())
                {
                    var lastPaymentDate = claim.Payments.Max(a => a.BenefitDate);
                    if(payPeriodDesc == "Weekly")
                    {
                        benefit.BenefitDate = lastPaymentDate.AddDays(7);
                    }
                    else if(payPeriodDesc == "Monthly")
                    {
                        benefit.BenefitDate = lastPaymentDate.AddMonths(1);
                    }
                    benefit.BenefitKey = -2;
                }
                return EditorFor(benefit);
            }
        }

        public IActionResult PaymentList(string claimId)
        {
            return PartialView(Context.UnemploymentBenefits.Where(a => a.ClaimId == claimId).ToList());
        }

        public IActionResult SavePayment(UnemploymentBenefit vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.BenefitKey < 0)
                {
                    if (Context.UnemploymentBenefits.Any(a => a.ClaimId == vm.ClaimId))
                    {
                        vm.BenefitKey = Context.UnemploymentBenefits.Where(a => a.ClaimId == vm.ClaimId).Max(a => a.BenefitKey) + 1;
                    }
                    else
                    {
                        vm.BenefitKey = 0;
                    }
                    Context.UnemploymentBenefits.Add(vm);
                }
                else
                {
                    var currentBenefit = Context.UnemploymentBenefits.First(a => a.ClaimId == vm.ClaimId && a.BenefitKey == vm.BenefitKey);
                    Context.Entry(currentBenefit).CurrentValues.SetValues(vm);
                }
                Context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, data = "Please make sure all fields are correctly filled out." });
            }
        }

        public IActionResult DeletePayment(string claimId, int benefitKey)
        {
            var payment = Context.UnemploymentBenefits.FirstOrDefault(a => a.ClaimId == claimId 
                && a.BenefitKey == benefitKey);
            
            if (payment == null)
            {
                return Json(new { success = false, data = "No claim exists with that ID." });
            }
            else
            {
                Context.UnemploymentBenefits.Remove(payment);
                Context.SaveChanges();
                return Json(new { success = true });
            }
        }
        #endregion
        #region Notes
        public IActionResult NotesList(string claimID)
        {
            return PartialView(Context.UnemploymentClaimNotes.Where(a => a.ClaimId == claimID).ToList());
        }
        public IActionResult CreateOrEditNote(string claimId, int? noteId)
        {
            var note = new UnemploymentClaimNotes()
            {
                ClaimId = claimId
            };
            if (noteId.HasValue)
            {
                note = Context.UnemploymentClaimNotes.FirstOrDefault(a => a.ClaimId == claimId && a.ClaimNoteId == noteId);
            }

            return EditorFor(note);
        }
        public IActionResult SaveNote(UnemploymentClaimNotes vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.ClaimNoteId == 0)
                {
                    vm.UserName = User.Identity.Name;
                    vm.InsertedDate = DateTime.Now;
                    Context.UnemploymentClaimNotes.Add(vm);
                }
                else
                {
                    var currentNote = Context.UnemploymentClaimNotes.Find(vm.ClaimNoteId);
                    Context.Entry(currentNote).CurrentValues.SetValues(vm);
                }
                Context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, data = "Please make sure all fields are correctly filled out." });
            }

        }
        public IActionResult DeleteNote(int noteId)
        {
            var note = Context.UnemploymentClaimNotes.Find(noteId);

            if (note == null)
            {
                return Json(new { success = false, data = "No claim exists with that ID." });
            }
            else
            {
                Context.UnemploymentClaimNotes.Remove(note);
                Context.SaveChanges();
                return Json(new { success = true });
            }
        }
        #endregion
        #endregion

    }
}
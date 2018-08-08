using System;
using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AtriumWebApp.Web.RiskManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.RiskManagement.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "RMU", Admin = true)]
    public class RMUnemploymentAdminController : BaseRiskManagementController
    {
        private const string AppCode = "RMU";
        protected new UnemploymentContext Context
        {
            get { return (UnemploymentContext)base.Context; }
        }
        public RMUnemploymentAdminController(IOptions<AppSettingsConfig> config, UnemploymentContext context) : base(config, context)
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
            SetStateDropDown(AppCode);
            SetPaymentPeriodDropDown(AppCode);
            //Get information for Community Table
            var appId = Context.Applications.Single(x => x.ApplicationCode == AppCode).ApplicationId;
            var comms = Context.Facilities.Where(a => a.AppCommInfo.Any(b => b.ApplicationId == appId));
            var appCommInfo = Context.ApplicationCommunityInfos.Where(apc => apc.ApplicationId == appId).ToList();
            SetLookbackDaysAdmin(HttpContext, AppCode);

            return View(new AdminViewModel
            {
                Communities = comms.ToList(),
                ApplicationCommunityInfos = appCommInfo,
                Regions = Context.MasterRegion.ToList()
            });
        }

        #region Other Post Backs
        [HttpPost]
        public ActionResult SavePayPeriod(string StateName, string PayPeriods)
        {
            var state = Context.States.Single(s => s.StateName == StateName);
            state.PaymentPeriodCd = PayPeriods;
            Context.SaveChanges();
            return RedirectToAction("");
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

        public void SetPaymentPeriodDropDown(string appCode)
        {
            var unempPayPeriodList = Context.PaymentPeriods.ToList();
            ViewData["PayPeriods"] = new SelectList(unempPayPeriodList, "PaymentPeriodCd", "PaymentPeriodDesc");

        }
        #endregion
    }
}

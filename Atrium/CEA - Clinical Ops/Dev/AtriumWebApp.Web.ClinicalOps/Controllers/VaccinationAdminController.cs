using System.Linq;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "VAC")]
    public class VaccinationAdminController : BaseAdminController
    {
        private const string AppCode = "VAC";

        public VaccinationAdminController(IOptions<AppSettingsConfig> config, VaccinationContext context) : base(config, context)
        {
        }

        protected new VaccinationContext Context
        {
            get { return (VaccinationContext)base.Context; }
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

            return View(new VaccinationAdminViewModel {
                AdminViewModel = adminViewModel,
                PayerGroups = Context.AtriumPayerGroups.ToList(),
                CommunityPayerGroupInfo = Context.PayerGroupInfos.Where(a => a.ApplicationId == adminViewModel.AppId).ToList()
            });
        }

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
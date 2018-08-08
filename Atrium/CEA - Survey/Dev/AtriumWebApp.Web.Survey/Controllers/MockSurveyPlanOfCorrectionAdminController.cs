
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;

using AtriumWebApp.Web.Survey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Survey.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "MSUPOC",Admin=true )]
    public class MockSurveyPlanOfCorrectionAdminController : BaseAdminController
    {
        private const string AppCode = "MSUPOC";

        public MockSurveyPlanOfCorrectionAdminController(IOptions<AppSettingsConfig> config, MockSurveyContext context) : base(config, context)
        {
        }

        protected new MockSurveyContext Context
        {
            get { return (MockSurveyContext)base.Context; }
        }


        public ActionResult Index()
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            var viewModel = CreateAdminViewModel(AppCode);
            return View(viewModel);
        }
    }
}
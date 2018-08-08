using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Utilization.Models;
using AtriumWebApp.Web.Utilization.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Utilization.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "PELI",Admin=true)]
    public class PELIAdminController : BaseAdminController
    {
        private const string AppCode = "PELI";
        private PELIController _peliController = null;
        private static object locker = new object();

        public PELIAdminController(IOptions<AppSettingsConfig> config, PELIContext context) : base(config, context)
        {
        }

        protected PELIController PELICTRLr
        {
            get
            {
                lock (locker)
                {
                    if (_peliController == null)
                        _peliController = new PELIController(Config,Context);
                    return _peliController;
                }
            }
        }

        protected new PELIContext Context
        {
            get { return (PELIContext)base.Context; }
        }

        public ActionResult Index()
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }

            SetLookbackDaysAdmin(HttpContext, AppCode);

            return View(new PELIAdminViewModel {
                AdminViewModel = CreateAdminViewModel(AppCode),
                PELITypes = PELICTRLr.GetALLPELITypeList()
            });
        }

        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, AppCode);
            return RedirectToAction("");
        }

        [HttpPost]
        public ActionResult CreatePELIType(PELIType newPELIType)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Some fields have errors, please double check values and submit again." });
            }
            newPELIType.SortOrder = PELICTRLr.NextSortOrder();
            Context.PELITypes.Add(newPELIType);
            Context.SaveChanges();
            return Json(new { Success = true, data = newPELIType });
        }


        [HttpPost]
        public JsonResult DeleteType(int id)
        {
            Context.PELITypes.Remove(Context.PELITypes.Find(id));
            Context.SaveChanges();
            return Json(new { Success = true });
        }

        [HttpPost]
        public JsonResult EditType(int id, string desc, int order, bool allowDataEntry)
        {
            var record = Context.PELITypes.Find(id);
            record.Description = desc;
            record.SortOrder = order;
            record.DataEntryFlg = allowDataEntry;
            Context.SaveChanges();
            return Json(new { Success = true });
        }

    }
}

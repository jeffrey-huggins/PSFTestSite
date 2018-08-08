using System;
using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Utilization.Models;
using AtriumWebApp.Web.Utilization.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Utilization.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "SKC", Admin=true)]
    public class SkilledChartingAdminController : BaseAdminController
    {
        private const string AppCode = "SKC";

        public SkilledChartingAdminController(IOptions<AppSettingsConfig> config, SkilledChartingContext context) : base(config, context)
        {
        }

        protected new SkilledChartingContext Context
        {
            get { return (SkilledChartingContext)base.Context; }
        }

        public ActionResult Index()
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }

            SetLookbackDaysAdmin(HttpContext, AppCode);

            return View(new SkilledChartingAdminViewModel
            {
                AdminViewModel = CreateAdminViewModel(AppCode),
                SkilledChartingGuidlines = Context.SkilledChartingGuidelines.OrderBy(g => g.SortOrder).ThenBy(g => g.GuidelineName).ToList()
            });
        }

        public ActionResult DocumentationQueues(int guidelineId)
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }

            var guideline = Context.SkilledChartingGuidelines.Find(guidelineId);
            if (guideline == null)
            {
                throw new ArgumentException(string.Format("SkilledChartingGuidelineId: '{0}' not found!", guidelineId));
            }

            return PartialView(guideline);
        }

        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, AppCode);
            return RedirectToAction("");
        }

        #region Guidelines
        [HttpPost]
        public ActionResult SaveGuideline(SkilledChartingGuideline model)
        {
            if (model.GuidelineId == 0) // New Guideline (Form POST)
            {

                model.DataEntryFlg = true;
                model.SortOrder = 0;
                model.InsertedDate = DateTime.Now;

                Context.SkilledChartingGuidelines.Add(model);
                Context.SaveChanges();

                return Json(new { Success = true, data = model });
            }
            else // Existing Guideline (AJAX)
            {
                var rec = Context.SkilledChartingGuidelines.Find(model.GuidelineId);
                if (rec == null)
                {
                    throw new ArgumentException(string.Format("SkilledChartingGuidelineId: '{0}' not found!", model.GuidelineId));
                }

                rec.GuidelineName = model.GuidelineName;
                rec.SortOrder = model.SortOrder;
                rec.LastModifiedDate = DateTime.Now;

                Context.SaveChanges();

                return Json(rec);
            }
        }

        [HttpPost]
        public ActionResult UpdateGuidelineDataEntryFlag(int guidelineId, bool dataEntryFlag)
        {
            var rec = Context.SkilledChartingGuidelines.Find(guidelineId);
            if (rec == null)
            {
                throw new ArgumentException(string.Format("SkilledChartingGuidelineId: '{0}' not found!", guidelineId));
            }

            rec.DataEntryFlg = dataEntryFlag;
            rec.LastModifiedDate = DateTime.Now;

            Context.SaveChanges();

            return new StatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult DeleteGuideline(int guidelineId)
        {
            var rec = Context.SkilledChartingGuidelines.Find(guidelineId);
            if (rec == null)
            {
                throw new ArgumentException(string.Format("SkilledChartingGuidelineId: '{0}' not found!", guidelineId));
            }

            Context.SkilledChartingGuidelines.Remove(rec);
            Context.SaveChanges();

            return new StatusCodeResult(200);
        }
        #endregion

        #region Documentation Queues
        [HttpPost]
        public ActionResult SaveDocumentationQueue(SkilledChartingDocumentationQueue model)
        {
            if (model.DocumentationQueueId == 0) // New Guideline (Form POST)
            {

                model.DocumentationQueueName = model.DocumentationQueueName.Replace("\r", "");
                model.DataEntryFlg = true;
                model.SortOrder = 0;
                model.InsertedDate = DateTime.Now;

                Context.SkilledChartingDocumentationQueues.Add(model);
                Context.SaveChanges();

                return Json(new { Success = true, data = model });
            }
            else // Existing Guideline (AJAX)
            {
                var rec = Context.SkilledChartingDocumentationQueues.Find(model.DocumentationQueueId);
                if (rec == null)
                {
                    throw new ArgumentException(string.Format("SkilledChartingDocumentationQueueId: '{0}' not found!", model.DocumentationQueueId));
                }

                rec.DocumentationQueueName = model.DocumentationQueueName.Replace("\r", "");
                rec.SortOrder = model.SortOrder;
                rec.LastModifiedDate = DateTime.Now;

                Context.SaveChanges();

                return Json(rec);
            }
        }

        [HttpPost]
        public ActionResult UpdateDocumentationQueueDataEntryFlag(int docQueueId, bool dataEntryFlag)
        {
            var rec = Context.SkilledChartingDocumentationQueues.Find(docQueueId);
            if (rec == null)
            {
                throw new ArgumentException(string.Format("SkilledChartingDocumentationQueueId: '{0}' not found!", docQueueId));
            }

            rec.DataEntryFlg = dataEntryFlag;
            rec.LastModifiedDate = DateTime.Now;

            Context.SaveChanges();

            return new StatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult DeleteDocumentationQueue(int docQueueId)
        {
            var rec = Context.SkilledChartingDocumentationQueues.Find(docQueueId);
            if (rec == null)
            {
                throw new ArgumentException(string.Format("SkilledChartingDocumentationQueueId: '{0}' not found!", docQueueId));
            }

            Context.SkilledChartingDocumentationQueues.Remove(rec);
            Context.SaveChanges();

            return new StatusCodeResult(200);
        }
        #endregion
    }
}
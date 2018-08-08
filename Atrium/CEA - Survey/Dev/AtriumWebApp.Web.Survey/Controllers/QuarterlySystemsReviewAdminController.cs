using System.Linq;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Survey.Models;
using AtriumWebApp.Web.Survey.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Survey.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "QSR",Admin=true)]
    public class QuarterlySystemsReviewAdminController : BaseAdminController
    {
        private const string AppCode = "QSR";

        public QuarterlySystemsReviewAdminController(IOptions<AppSettingsConfig> config, QuarterlySystemsReviewContext context) : base(config, context)
        {
        }

        protected new QuarterlySystemsReviewContext Context
        {
            get { return (QuarterlySystemsReviewContext)base.Context; }
        }


        public ActionResult Index()
        {
            //If user doesn't have access, redirect to the home page
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            var adminViewModel = CreateAdminViewModel(AppCode);
            var socMeasureData = Context.StandardsOfCareMeasures.OrderBy(m => m.Name);
            var socQuestionData = Context.StandardsOfCareQuestions.OrderBy(m => m.ReviewMeasure.Name).ThenBy(m => m.SortOrder).ThenBy(m => m.Text);
            var generalMeasureData = Context.GeneralMeasures.OrderBy(m => m.SortOrder).ThenBy(m => m.Name);
            var generalQuestionData = Context.GeneralQuestions.OrderBy(m => m.ReviewMeasure.Name).ThenBy(m => m.SortOrder).ThenBy(m => m.Text);
            var viewModel = new ReviewAdminViewModel
            {
                AdminViewModel = adminViewModel,
                StandardsOfCareMeasures = socMeasureData.Select(m => new StandardsOfCareMeasureViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    ThresholdBonusPoints = m.ThresholdBonusScore,
                    SortOrder = m.SortOrder
                }).ToList(),
                StandardsOfCareQuestions = socQuestionData.Select(m => new ReviewQuestionViewModel
                {
                    Id = m.Id,
                    MeasureId = m.ReviewMeasureId,
                    MeasureName = m.ReviewMeasure.Name,
                    Text = m.Text,
                    MaxPoints = m.MaxScore,
                    SortOrder = m.SortOrder
                }).ToList(),
                GeneralMeasures = generalMeasureData.Select(m => new GeneralMeasureViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    RequiresPatientSample = m.RequiresPatientSample,
                    SortOrder = m.SortOrder
                }).ToList(),
                GeneralQuestions = generalQuestionData.Select(m => new ReviewQuestionViewModel
                {
                    Id = m.Id,
                    MeasureId = m.ReviewMeasureId,
                    MeasureName = m.ReviewMeasure.Name,
                    Text = m.Text,
                    MaxPoints = m.MaxScore,
                    SortOrder = m.SortOrder
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditStandardsOfCareMeasure(StandardsOfCareMeasureViewModel viewModel)
        {
            var measure = Context.StandardsOfCareMeasures.Find(viewModel.Id);
            if (measure == null)
            {
                return NotFound("Could not find standards of care measure with specified Id.");
            }
            measure.Name = viewModel.Name;
            measure.ThresholdBonusScore = viewModel.ThresholdBonusPoints;
            measure.SortOrder = viewModel.SortOrder;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public ActionResult EditGeneralMeasure(GeneralMeasureViewModel viewModel)
        {
            var measure = Context.GeneralMeasures.Find(viewModel.Id);
            if (measure == null)
            {
                return NotFound("Could not find general measure with specified Id.");
            }
            measure.Name = viewModel.Name;
            measure.RequiresPatientSample = viewModel.RequiresPatientSample;
            measure.SortOrder = viewModel.SortOrder;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public ActionResult EditStandardsOfCareQuestion(ReviewQuestionViewModel viewModel)
        {
            var question = Context.StandardsOfCareQuestions.Find(viewModel.Id);
            if (question == null)
            {
                return NotFound("Could not find question with specified Id.");
            }
            EditReviewQuestion(question, viewModel);
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public ActionResult EditGeneralQuestion(ReviewQuestionViewModel viewModel)
        {
            var question = Context.GeneralQuestions.Find(viewModel.Id);
            if (question == null)
            {
                return NotFound("Could not find question with specified Id.");
            }
            EditReviewQuestion(question, viewModel);
            return Json(new SaveResultViewModel { Success = true });
        }

        private void EditReviewQuestion(BaseReviewQuestion question, ReviewQuestionViewModel viewModel)
        {
            question.Text = viewModel.Text;
            question.MaxScore = viewModel.MaxPoints;
            question.SortOrder = viewModel.SortOrder;
            Context.SaveChanges();
        }
    }
}

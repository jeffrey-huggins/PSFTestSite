using AtriumWebApp.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewAdminViewModel
    {
        public AdminViewModel AdminViewModel { get; set; }
        public IList<StandardsOfCareMeasureViewModel> StandardsOfCareMeasures { get; set; }
        public IList<ReviewQuestionViewModel> StandardsOfCareQuestions { get; set; }
        public IList<GeneralMeasureViewModel> GeneralMeasures { get; set; }
        public IList<ReviewQuestionViewModel> GeneralQuestions { get; set; }
    }
}
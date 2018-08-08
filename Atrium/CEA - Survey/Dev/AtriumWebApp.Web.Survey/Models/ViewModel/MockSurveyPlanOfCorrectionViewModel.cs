using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class MockSurveyPlanOfCorrectionViewModel : BaseMockSurveyViewModel
    {
        [DisplayName("Follow-up Date")]
        public DateTime? FollowUpDate { get; set; }
        [DisplayName("Complete?")]
        public bool Close { get; set; }
        public IList<MockSurveyPlanOfCorrectionGroupViewModel> FederalGroups { get; set; }
        public IList<MockSurveyPlanOfCorrectionGroupViewModel> SafetyGroups { get; set; }
    }
}
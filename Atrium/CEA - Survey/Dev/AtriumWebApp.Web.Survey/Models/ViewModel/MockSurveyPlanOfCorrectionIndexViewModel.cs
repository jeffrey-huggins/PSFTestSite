using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class MockSurveyPlanOfCorrectionIndexViewModel
    {
        public IList<MockSurvey> Items { get; set; }
        [DisplayName("Community")]
        public int CurrentCommunity { get; set; }
        public IList<Community> Communities { get; set; }
        [DisplayName("From")]
        public DateTime OccurredRangeFrom { get; set; }
        [DisplayName("To")]
        public DateTime OccurredRangeTo { get; set; }
        public MockSurveyPlanOfCorrectionViewModel PlanOfCorrection { get; set; }
        public bool IsAdministrator { get; set; }
    }
}
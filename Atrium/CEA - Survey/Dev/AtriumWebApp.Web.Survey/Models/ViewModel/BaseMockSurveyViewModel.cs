using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class BaseMockSurveyViewModel
    {
        public int MockSurveyId { get; set; }

        [DisplayName("Open Date")]
        public DateTime MockSurveyDate { get; set; }
        public DateTime? ClosedDate { get; set; }
    }
}
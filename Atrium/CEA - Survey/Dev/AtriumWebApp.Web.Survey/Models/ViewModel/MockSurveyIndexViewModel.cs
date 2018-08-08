using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class MockSurveyIndexViewModel
    {
        public IList<MockSurvey> Items { get; set; }
        [DisplayName("Community")]
        public int CurrentCommunity { get; set; }
        public IList<Community> Communities { get; set; }
        [DisplayName("From")]
        public DateTime OccurredRangeFrom { get; set; }
        [DisplayName("To")]
        public DateTime OccurredRangeTo { get; set; }
        public BaseMockSurveyViewModel Current { get; set; }
        public bool CanEditClosed { get; set; }
        public bool CanDeleteClosed { get; set; }
        //public bool IsAdministrator { get; set; }
    }
}
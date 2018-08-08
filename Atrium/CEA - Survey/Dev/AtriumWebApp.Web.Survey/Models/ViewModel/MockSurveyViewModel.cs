using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class MockSurveyViewModel : BaseMockSurveyViewModel
    {
        public int CommunityId { get; set; }
        [RequiredIf("Close", true)]
        [DisplayName("Follow-up Date")]
        public DateTime? FollowUpDate { get; set; }
        [StringLength(1024)]
        [RequiredIf("Close", true)]
        [DisplayName("Team Members")]
        public string TeamMembers { get; set; }
        [StringLength(128)]
        [RequiredIf("Close", true)]
        [DisplayName("Signature")]
        public string CloseSignature { get; set; }
        [DisplayName("Close?")]
        public bool Close { get; set; }
        public bool CanEditClosed { get; set; }
        
        //public bool IsAdministrator { get; set; }

        public IList<MockSurveyGroupViewModel> FederalGroups { get; set; }
        public IList<MockSurveyGroupViewModel> SafetyGroups { get; set; }
    }
}
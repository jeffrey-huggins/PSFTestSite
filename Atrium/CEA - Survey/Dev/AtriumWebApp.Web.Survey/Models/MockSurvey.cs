using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models
{
    public class MockSurvey
    {
        public MockSurvey()
        {
            MockSurveyDate = DateTime.Today;
        }

        public int Id { get; set; }
        public DateTime MockSurveyDate { get; set; }
        [StringLength(1024)]
        [RequiredIf("IsClosed", true)]
        public string TeamMembers { get; set; }
        public DateTime? ClosedDate { get; set; }
        [StringLength(128)]
        [RequiredIf("IsClosed", true)]
        public string ClosedSignature { get; set; }
        [RequiredIf("IsClosed", true)]
        public DateTime? FollowUpDate { get; set; }
        [RequiredIf("IsFollowUpComplete", true)]
        public DateTime? PlanOfCorrectionCompleteDate { get; set; }
        public DateTime? FollowUpCompleteDate { get; set; }

        public bool IsClosed
        {
            get { return ClosedDate.HasValue; }
        }

        public bool IsPlanOfCorrectionComplete
        {
            get { return PlanOfCorrectionCompleteDate.HasValue; }
        }

        public bool IsFollowUpComplete
        {
            get { return FollowUpCompleteDate.HasValue; }
        }

        public int CommunityId { get; set; }
        public Community Community { get; set; }

        public ICollection<MockFederalCitation> MockFederalCitations { get; set; }
        public ICollection<MockSafetyCitation> MockSafetyCitations { get; set; }
    }
}
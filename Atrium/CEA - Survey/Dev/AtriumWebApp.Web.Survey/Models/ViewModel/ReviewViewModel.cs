using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewViewModel
    {
        public bool CanCloseDietary { get; set; }
        public bool CanCloseNursing { get; set; }
        public bool CanEditDate { get; set; }
        public bool CanEdiit { get; set; }
        public bool CanDelete { get; set; }
        //public bool IsAdministrator { get; set; }
        public int ReviewId { get; set; }
        public int CommunityId { get; set; }
        public string CommunityName { get; set; }
        [DisplayName("From")]
        [MatchesCalendarQuarterAttribute("ReviewDate", "Review Date")]
        public DateTime BeginSampleDate { get; set; }
        [DisplayName("To")]
        [MatchesCalendarQuarterAttribute("ReviewDate", "Review Date")]
        [DateGreaterThanAttribute("BeginSampleDate")]
        public DateTime EndSampleDate { get; set; }
        [DisplayName("Date")]
        public DateTime ReviewDate { get; set; }
        [DisplayName("Validated by Nurse")]
        public bool CloseNursing { get; set; }
        [StringLength(128)]
        [DisplayName("Signature")]
        [RequiredIf("CloseNursing")]
        public string CloseNursingSignature { get; set; }
        [DisplayName("Validated by Dietician")]
        public bool CloseDietary { get; set; }
        [StringLength(128)]
        [DisplayName("Signature")]
        [RequiredIf("CloseDietary")]
        public string CloseDietarySignature { get; set; }
        public int PointAdjustment { get; set; }

        public IEnumerable<ReviewSectionViewModel> Sections { get; set; }

        public string Quarter
        {
            get { return ReviewDate.ToCalendarQuarterString(); }
        }

        public double TotalEarnedPoints
        {
            get
            {
                if (Sections == null)
                {
                    return 0;
                }
                return Sections.Sum(q => q.EarnedPoints) + PointAdjustment;
            }
        }

        public double TotalPossiblePoints
        {
            get
            {
                if (Sections == null)
                {
                    return 0;
                }
                return Sections.Sum(q => q.PossiblePoints);
            }
        }
    }
}

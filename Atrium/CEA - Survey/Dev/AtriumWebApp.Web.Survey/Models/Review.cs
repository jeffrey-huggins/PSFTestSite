using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models
{
    public class Review
    {
        public int Id { get; set; }
        public DateTime ReviewDate { get; set; }
        [MatchesCalendarQuarterAttribute("ReviewDate")]
        public DateTime BeginSampleDate { get; set; }
        [MatchesCalendarQuarterAttribute("ReviewDate")]
        [DateGreaterThanAttribute("BeginSampleDate")]
        public DateTime EndSampleDate { get; set; }
        public bool IsClosedByNurse { get; set; }
        public DateTime? ClosedByNurseDate { get; set; }
        [StringLength(128)]
        public string ClosedByNurseSignature { get; set; }
        public bool IsClosedByDietitian { get; set; }
        public DateTime? ClosedByDietitianDate { get; set; }
        [StringLength(128)]
        public string ClosedByDietitianSignature { get; set; }
        public bool IsClosed
        {
            get { return IsClosedByNurse && IsClosedByDietitian; }
        }
        
        public int CommunityId { get; set; }
        public Community Community { get; set; }

        public ICollection<StandardsOfCareSection> StandardsOfCareSections { get; set; }
        public ICollection<GeneralSection> GeneralSections { get; set; }
        public ICollection<AdditionalReviewAnswer> AdditionalAnswers { get; set; }
    }
}
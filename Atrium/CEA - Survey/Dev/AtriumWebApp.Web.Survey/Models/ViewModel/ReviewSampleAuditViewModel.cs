using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewSampleAuditViewModel : BaseReviewSampleViewModel
    {
        public int Id { get; set; }
        [StringLength(128)]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
        public bool IsResidentSample { get; set; }
        public string SectionName { get; set; }

        public IList<ReviewAnswerViewModel> Answers { get; set; }
    }
}
using System.Collections.Generic;

namespace AtriumWebApp.Web.Survey.Models
{
    public class StandardsOfCareSample : BaseReviewSample
    {
        public int ReviewSectionId { get; set; }
        public StandardsOfCareSection ReviewSection { get; set; }

        public ICollection<StandardsOfCareAnswer> ReviewAnswers { get; set; }
    }
}
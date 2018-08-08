using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewAdditionalAuditViewModel
    {
        public int ReviewId { get; set; }
        public IList<ReviewAnswerViewModel> Answers { get; set; }
    }
}
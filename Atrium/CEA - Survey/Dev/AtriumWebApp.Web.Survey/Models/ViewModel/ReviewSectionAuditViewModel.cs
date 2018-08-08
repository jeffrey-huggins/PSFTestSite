using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewSectionAuditViewModel
    {
        public int SectionId { get; set; }
        public bool RequiresPatientSample { get; set; }
        public ReviewSectionType SectionType { get; set; }
        public IEnumerable<ReviewSampleAuditViewModel> SampleAudits { get; set; }
    }
}
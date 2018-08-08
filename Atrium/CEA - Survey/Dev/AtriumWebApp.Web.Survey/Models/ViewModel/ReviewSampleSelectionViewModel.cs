using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class ReviewSampleSelectionViewModel
    {
        public int SectionId { get; set; }
        public ReviewSectionType SectionType { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public IEnumerable<ReviewSampleOptionViewModel> Options { get; set; }
    }
}

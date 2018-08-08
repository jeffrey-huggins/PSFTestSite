using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class CitationIdentityViewModel
    {
        public int? CitationId { get; set; }
        [Required]
        [ContainedInEnumerable("ValidGroupTypes", true)]
        public string GroupType { get; set; }

        public static IReadOnlyCollection<string> ValidGroupTypes
        {
            get { return DeficiencyGroup.ValidGroupTypes; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class DeficiencyGroupViewModel : EditDeficiencyGroupViewModel
    {
        [Required]
        [StringLength(10)]
        [ContainedInEnumerable("ValidGroupTypes", true)]
        public string GroupType { get; set; }

        public static IReadOnlyCollection<string> ValidGroupTypes
        {
            get { return DeficiencyGroup.ValidGroupTypes; }
        }
    }
}
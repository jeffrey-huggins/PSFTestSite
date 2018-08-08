using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models
{
    public class DeficiencyGroup
    {
        private static readonly IReadOnlyCollection<string> _ValidGroupTypes = new ReadOnlyCollection<string>(new[] { "Federal", "Safety" });

        public int Id { get; set; }
        [Required]
        [StringLength(10)]
        [ContainedInEnumerable("ValidGroupTypes", true)]
        public string GroupType { get; set; }
        [Required]
        [StringLength(64)]
        public string Description { get; set; }
        public int SortOrder { get; set; }

        public ICollection<FederalDeficiency> FederalDeficiencies { get; set; }
        public ICollection<SafetyDeficiency> SafetyDeficiencies { get; set; }

        public static IReadOnlyCollection<string> ValidGroupTypes
        {
            get { return _ValidGroupTypes; }
        }
    }
}

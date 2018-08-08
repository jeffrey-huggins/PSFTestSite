using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models
{
    public class FederalDeficiency : BaseDeficiency
    {
        public int? DeficiencyGroupId { get; set; }

        public DeficiencyGroup DeficiencyGroup { get; set; }

        [StringLength(4096)]
        public override string Instructions { get; set; }

        [Required]
        [StringLength(3)]
        public string AtriumPayerGroupCode { get; set; }
        public AtriumPayerGroup AtriumPayerGroup { get; set; }
    }
}
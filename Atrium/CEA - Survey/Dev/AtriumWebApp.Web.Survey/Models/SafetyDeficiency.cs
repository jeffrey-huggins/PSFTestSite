using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public class SafetyDeficiency : BaseDeficiency
    {
        public int? DeficiencyGroupId { get; set; }
        public DeficiencyGroup DeficiencyGroup { get; set; }

        [StringLength(4096)]
        public override string Instructions { get; set; }
    }
}
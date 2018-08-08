using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Models
{
    public class AtriumPayerGroup
    {
        [Key]
        [Required]
        [StringLength(3)]
        public string AtriumPayerGroupCode { get; set; }
        [Required]
        [StringLength(80)]
        public string AtriumPayerGroupName { get; set; }
        public bool IsCommunitySurveyEligible { get; set; }
    }
}
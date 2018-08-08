using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public class ScopeAndSeverity
    {
        [Key]
        public int SASId { get; set; }
        [StringLength(2)]
        [Required]
        public string SASCode { get; set; }
        [StringLength(15)]
        [Required]
        public string Scope { get; set; }
        [Required]
        public int SeverityLevel { get; set; }
    }
}
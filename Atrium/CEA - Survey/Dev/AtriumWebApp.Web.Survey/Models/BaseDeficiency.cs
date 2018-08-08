using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public abstract class BaseDeficiency
    {
        public int Id { get; set; }
        [Required]
        [StringLength(20)]
        public string TagCode { get; set; }
        [Required]
        [StringLength(4096)]
        public string Description { get; set; }
        public abstract string Instructions { get; set; }
    }
}
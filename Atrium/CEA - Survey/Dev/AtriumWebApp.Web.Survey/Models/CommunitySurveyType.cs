using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Web.Survey.Models
{
    public class CommunitySurveyType
    {
        [Key]
        public int SurveyTypeId { get; set; }
        [StringLength(30)]
        [Required]
        public string SurveyTypeDesc { get; set; }
        public bool ComplaintFlg { get; set; }
    }
}
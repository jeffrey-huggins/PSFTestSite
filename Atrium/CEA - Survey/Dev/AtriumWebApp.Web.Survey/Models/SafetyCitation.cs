using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public class SafetyCitation : BaseCitation
    {
        public int SafetyDeficiencyId { get; set; }
        public int? SASId { get; set; }
        public bool WaiverFlg { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public class FederalCitation : BaseCitation
    {
        public int FederalDeficiencyId { get; set; }
        public int SASId { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models
{
    public class StateCitation : BaseCitation
    {
        public int StateDeficiencyId { get; set; }
        public int? SASId { get; set; }
    }
}
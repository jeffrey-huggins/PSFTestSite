using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class CloseIncidentAllCommunity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeId { get; set; }
    }
}
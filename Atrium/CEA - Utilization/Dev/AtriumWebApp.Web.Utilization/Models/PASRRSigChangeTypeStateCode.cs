using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PASRRSigChangeTypeStateCode
    {
        [Key]
        public int SigChangeTypeId { get; set; }
        
        [Key]
        public string StateCode { get; set; }
    }
}
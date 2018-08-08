using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PASRRTypeStateCode
    {
        [Key]
        public int PASRRTypeId { get; set; }

        [Key]
        public string StateCode { get; set; }
    }
}
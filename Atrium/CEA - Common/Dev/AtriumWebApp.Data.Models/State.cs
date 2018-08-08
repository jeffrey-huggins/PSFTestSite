using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class State
    {
        [Key]
        public string StateCd { get; set; }
        public string StateName { get; set; }
        public string PaymentPeriodCd { get; set; }
    }
}
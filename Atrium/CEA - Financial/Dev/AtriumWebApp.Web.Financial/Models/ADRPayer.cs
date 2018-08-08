using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Web.Financial.Models
{
    public class ADRPayer
    {
        [Key]
        public int ADRPayerId { get; set; }
        public string ADRPayerName { get; set; }
    }
}

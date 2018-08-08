using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class SystemLogUsage
    {
        [Key]
        public int UsageID { get; set; }
        public int ApplicationId { get; set; }
        public string ADDomainName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
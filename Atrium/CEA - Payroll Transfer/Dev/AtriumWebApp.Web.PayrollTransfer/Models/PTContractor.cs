using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.PayrollTransfer.Models
{
    [Serializable]
    public class PTContractor
    {
        [Key]
        public int PTContractorId { get; set; }
        [Required]
        [MaxLength(60)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(60)]
        public string LastName { get; set; }
        [MaxLength(20)]
        public string VendorNbr { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public ICollection<Community> Communities { get; set; }
    }
}
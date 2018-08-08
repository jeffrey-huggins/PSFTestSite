using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ProviderAddressType : ContractBase
    {
        [Required]
        [StringLength(32)]
        public string Name { get; set; }

        public ICollection<ProviderAddress> Addresses { get; set; }
    }
}
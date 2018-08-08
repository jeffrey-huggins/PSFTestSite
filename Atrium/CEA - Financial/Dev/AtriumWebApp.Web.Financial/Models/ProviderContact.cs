using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ProviderContact : ContractBase
    {
        public int ContractProviderId { get; set; }
        public int ProviderContactTypeId { get; set; }
        public ProviderContactType ContactType { get; set; }
        public ContractProvider Provider { get; set; }

        [StringLength(32)]
        public string Title { get; set; }

        [Phone]
        [StringLength(16)]
        public string CellPhone { get; set; }

        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Phone]
        [StringLength(16)]
        public string Phone { get; set; }

        [Phone]
        [StringLength(16)]
        public string Fax { get; set; }

        [EmailAddress]
        [StringLength(128)]
        public string Email { get; set; }
    }
}
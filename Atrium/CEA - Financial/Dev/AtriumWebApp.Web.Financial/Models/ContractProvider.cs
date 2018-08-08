using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ContractProvider : ContractBase
    {
        public bool IsActive { get; set; }
        
        [StringLength(128)]
        [Required]
        public string Name { get; set; }

        [StringLength(16)]
        public string LicenseId { get; set; }
        public DateTime? LicenseExpirationDate { get; set; }

        [StringLength(16)]
        public string InsurancePolicyId { get; set; }
        [StringLength(128)]
        public string InsuranceCompany { get; set; }
        public DateTime? InsuranceExpirationDate { get; set; }
        
        public ICollection<ProviderDocument> Documents { get; set; }
        public ICollection<ProviderAddress> Addresses { get; set; }
        public ICollection<ProviderContact> Contacts { get; set; }

        public ICollection<Contract> Contracts { get; set; }

        public ContractProvider()
        {
            Documents = new List<ProviderDocument>();
            Addresses = new List<ProviderAddress>();
            Contacts = new List<ProviderContact>();
            Contracts = new List<Contract>();
        }
    }
}
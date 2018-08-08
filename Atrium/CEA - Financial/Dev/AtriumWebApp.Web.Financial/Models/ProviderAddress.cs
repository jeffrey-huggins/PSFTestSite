using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ProviderAddress : ContractBase
    {
        public int ContractProviderId { get; set; }
        public int ProviderAddressTypeId { get; set; }
        public ProviderAddressType AddressType { get; set; }
        public ContractProvider Provider { get; set; }

        [StringLength(128)]
        public string AddressLineOne { get; set; }
        [StringLength(128)]
        public string AddressLineTwo { get; set; }
        [StringLength(128)]
        [Required]
        public string City { get; set; }
        [StringLength(128)]
        public string County { get; set; }

        [StringLength(20)]
        public string ZipCode { get; set; }

        public string StateId { get; set; }
        public State State { get; set; }
    }
}
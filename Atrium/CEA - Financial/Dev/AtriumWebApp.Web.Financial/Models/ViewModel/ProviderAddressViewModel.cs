using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models.ViewModel
{
    public class ProviderAddressViewModel
    {
        public int Id { get; set; }
        public int ContractProviderId { get; set; }
        public int AddressTypeId { get; set; }

        [StringLength(128)]
        public string AddressLineOne { get; set; }

        [StringLength(128)]
        public string AddressLineTwo { get; set; }

        [Required]
        [StringLength(128)]
        public string City { get; set; }

        [StringLength(128)]
        public string County { get; set; }

        [StringLength(20)]
        public string ZipCode { get; set; }

        public List<State> States { get; set; }
        public List<ProviderAddressType> AddressTypes { get; set; }
        public string StateId { get; set; }

        public ProviderAddressViewModel()
        {
            AddressTypes = new List<ProviderAddressType>();
            States = new List<State>();
        }
    }
}
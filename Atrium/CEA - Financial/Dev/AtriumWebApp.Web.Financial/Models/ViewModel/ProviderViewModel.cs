using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models.ViewModel
{
    public class ProviderViewModel 
    {
        public int Id { get; set; }

        //TODO: Remove this...Community is tied to Contract now
        public int SelectedCommunityId { get; set; }

        [Required]
        [StringLength(128)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [StringLength(16)]
        [DisplayName("License Id")]
        public string LicenseId { get; set; }

        [DisplayName("License Expire Date")]
        public DateTime? LicenseExpirationDate { get; set; }

        [StringLength(16)]
        [DisplayName("Insurance Policy Id")]
        public string InsurancePolicyId { get; set; }

        [StringLength(128)]
        [DisplayName("Insurance Company")]
        public string InsuranceCompany { get; set; }

        [DisplayName("Insurance Expire Date")]
        public DateTime? InsuranceExpirationDate { get; set; }

        public ICollection<ProviderAddressViewModel> Addresses { get; set; }
        public ICollection<ProviderContactViewModel> Contacts { get; set; }
        public ICollection<ProviderDocumentViewModel> Documents { get; set; }
        public ICollection<ContractViewModel> Contracts { get; set; }

        public ProviderViewModel()
        {
            Addresses = new List<ProviderAddressViewModel>();
            Contacts = new List<ProviderContactViewModel>();
            Contracts = new List<ContractViewModel>();
            Documents = new List<ProviderDocumentViewModel>();
        }
    }
}
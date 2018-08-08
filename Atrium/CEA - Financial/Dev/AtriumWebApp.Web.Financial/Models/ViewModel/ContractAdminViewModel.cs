using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models.ViewModel
{
    public class ContractAdminViewModel
    {
        public List<Community> Facilities { get; set; }

        [DisplayName("Address Types")]
        public List<ProviderAddressType> AddressTypes { get; set; }

        [DisplayName("Contact Types")]
        public List<ProviderContactType> ContactTypes { get; set; }

        [DisplayName("Current Address Type")]
        public ProviderAddressType CurrentAddressType { get; set; }

        [DisplayName("Current Contact Type")]
        public ProviderContactType CurrentContactType { get; set; }


        public int SelectedCategoryId { get; set; }

        [DisplayName("Categories")]
        public List<ContractCategory> Categories { get; set; }

        [DisplayName("Sub Categories")]
        public List<ContractSubCategory> SubCategories { get; set; }

        [DisplayName("Current Category")]
        public ContractCategory CurrentCategory { get; set; }

        [DisplayName("Current Sub Category")]
        public ContractSubCategory CurrentSubCategory { get; set; }


        [DisplayName("Renewal Types")]
        public List<ContractRenewal> RenewalTypes { get; set; }

        [DisplayName("Termination Notices")]
        public List<ContractTerminationNotice> TerminationNotices { get; set; }

        [DisplayName("Payment Terms")]
        public List<ContractPaymentTerm> PaymentTerms { get; set; }

        [DisplayName("Current Renewal Type")]
        public ContractRenewal CurrentRenewalType { get; set; }

        [DisplayName("Current Termination Notice")]
        public ContractTerminationNotice CurrentTerminationNotice { get; set; }

        [DisplayName("Current Payment Term")]
        public ContractPaymentTerm CurrentPaymentTerm { get; set; }


    }
}
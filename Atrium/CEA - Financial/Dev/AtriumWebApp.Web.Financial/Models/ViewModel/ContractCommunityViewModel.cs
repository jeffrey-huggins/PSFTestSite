using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models.ViewModel
{
    public class ContractCommunityViewModel 
    {
        //DDL Lists
        public ICollection<Community> Communities { get; set; }
        public ICollection<ContractPaymentTerm> PaymentTerms { get; set; }
        public ICollection<ContractRenewal> Renewals { get; set; }
        public ICollection<ContractTerminationNotice> TerminationNotices { get; set; } 

        public int ContractInfoId { get; set; }
        public int CommunityId { get; set; }

        public int ContractPaymentTermId { get; set; }
        public int ContractRenewalId { get; set; }
        public int ContractTerminationNoticeId { get; set; }

        public bool ArchiveFlg { get; set; }
        public DateTime? ArchiveDate { get; set; }

        [DisplayName("Credentialing Date")]
        public DateTime? CredentialingDate { get; set; }

        [DisplayName("Re-Credentialing Date")]
        public DateTime? ReCredentialingDate { get; set; }

        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

    }
}
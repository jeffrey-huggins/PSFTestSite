using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ContractInfoCommunity
    {
        public int ContractInfoId { get; set; }
        public int CommunityId { get; set; }
        public int ContractPaymentTermId { get; set; }
        public int ContractRenewalId { get; set; }
        public int ContractTerminationNoticeId { get; set; }

        public Contract ContractInfo { get; set; }
        public Community Community { get; set; }
        public ContractPaymentTerm PaymentTerm { get; set; }
        public ContractRenewal Renewal { get; set; }
        public ContractTerminationNotice TerminationNotice { get; set; }

        public bool ArchiveFlg { get; set; }
        public DateTime? ArchiveDate { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CredentialingDate { get; set; }
        public DateTime? ReCredentialingDate { get; set; }
    }
}
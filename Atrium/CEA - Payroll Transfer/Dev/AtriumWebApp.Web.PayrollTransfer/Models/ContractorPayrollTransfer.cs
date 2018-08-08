using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.PayrollTransfer.Models
{
    [Serializable]
    public class ContractorPayrollTransfer
    {
        public int ContractorPayrollTransferId { get; set; }
        public int PTContractorId { get; set; }
        public int CommunityId { get; set; }
        public int GeneralLedgerId { get; set; }
        public DateTime TransferDate { get; set; }
        public decimal HourCnt { get; set; }
        public decimal PayAmt { get; set; }
        public bool PBJOnlyFlg { get; set; }
        public bool DeletedFlg { get; set; }
        public DateTime? DeletedTS { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
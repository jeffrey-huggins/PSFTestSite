using AtriumWebApp.Models;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.PayrollTransfer.Models
{
    [Serializable]
    public class EmployeePayrollTransfer
    {
        [Key]
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int SourceCommunityId { get; set; }
        public int SourceGeneralLedgerId { get; set; }
        public DateTime TransferDate { get; set; }
        public int DestinationGeneralLedgerId { get; set; }
        public int DestinationCommunityId { get; set; }
        public decimal HourCnt { get; set; }
        public decimal PayAmt { get; set; }
        public string PayType { get; set; }
        public bool PBJOnlyFlg { get; set; }
        public bool DeletedFlg { get; set; }
        public DateTime? DeletedTS { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        [JsonIgnore]
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        [JsonIgnore]
        [ForeignKey("SourceGeneralLedgerId")]
        public virtual GeneralLedgerAccount SourceLedger { get; set; }
        [JsonIgnore]
        [ForeignKey("DestinationGeneralLedgerId")]
        public virtual GeneralLedgerAccount DestinationLedger { get; set; }
        [JsonIgnore]
        [ForeignKey("SourceCommunityId")]
        public virtual Community SourceCommunity { get; set; }
        [JsonIgnore]
        [ForeignKey("DestinationCommunityId")]
        public virtual Community DestinationCommunity { get; set; }

    }
}
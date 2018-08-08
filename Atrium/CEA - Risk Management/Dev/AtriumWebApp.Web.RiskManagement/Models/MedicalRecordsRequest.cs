using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class MedicalRecordsRequest
    {
        [Key]
        public string Requestid { get; set; }
        public int PatientId { get; set; }
        [DisplayName("Lawsuit Filed")]
        public bool LawSuitFiledFlg { get; set; }
        [DisplayName("Arbitration Agreement")]
        public bool ArbitrationAgreementFlg { get; set; }
        [DisplayName("Request From:")]
        public string RequestSource { get; set; }
        [DisplayName("Requested Documents:")]
        public string RequestDocument { get; set; }
        [DisplayName("Date Opened:")]
        public DateTime OpenDate { get; set; }
        [DisplayName("Date Closed:")]
        public DateTime? ClosedDate { get; set; }
        [DisplayName("Date Requested:")]
        public DateTime? FileRequestedDate { get; set; }
        [DisplayName("Date Received:")]
        public DateTime? FileReceivedDate { get; set; }
        [DisplayName("Acknowledgement Sent:")]
        public DateTime? AcknowledgementSentDate { get; set; }
        [DisplayName("Payment Sent:")]
        public DateTime? PaymentSentDate { get; set; }
        [DisplayName("Payment Received:")]
        public DateTime? PaymentReceivedDate { get; set; }
        [DisplayName("Plaintiff Counsel:")]
        public string PlaintiffCounsel { get; set; }
        [DisplayName("Defense Counsel:")]
        public string DefenseCounsel { get; set; }
        [DisplayName("Complaint")]
        public string Complaint { get; set; }
        [DisplayName("Death Claim:")]
        public bool DeathClaim { get; set; }
        [DisplayName("Lawsuit Filed:")]
        public DateTime? LawSuitFiledDate { get; set; }
        [DisplayName("Trial Date:")]
        public DateTime? TrialDate { get; set; }
        [DisplayName("Settlement Date:")]
        public DateTime? SettlementDate { get; set; }
        [DisplayName("Closed Date:")]
        public DateTime? LawSuitClosedDate { get; set; }
        public string LastUser { get; set; }

        public virtual List<MedicalRecordsRequestDocument> Documents { get; set; }
        public virtual List<MedicalRecordsRequestNotes> Notes { get; set; }
    }
}
using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.PayrollTransfer.Models.ViewModel
{
    public class ContractorPayrollEntryViewModel
    {
        public int Id { get; set; }
        public int ContractorId { get; set; }
        public int SourceCommunityId { get; set; }
        public int SourceGeneralLedgerId { get; set; }
        public DateTime TransferDate { get; set; }
        //public int DestinationGeneralLedgerId { get; set; }
        //public int DestinationCommunityId { get; set; }
        public decimal HourCnt { get; set; }
        public decimal PayAmt { get; set; }
        public string PayType { get; set; }
        public bool PBJOnlyFlg { get; set; }


        public IList<PTContractor> Contractors { get; set; }
        public PTContractor Contractor { get; set; }
        public IList<GeneralLedgerAccount> GLAccounts { get; set; }
        public GeneralLedgerAccount SourceGLAccount { get; set; }
        //public GLAccountVewModel DestinationGLAccount { get; set; }
        public string ContractorName { get { return Contractor.LastName + ", " + Contractor.FirstName; } }


        public IList<Community> Communities { get; set; }
        public Community SourceCommunity { get; set; }
        public Community DestinationCommunity { get; set; }
        

        public bool IsAdministrator { get; set; }
        public string AppCode { get; set; }
    }
}
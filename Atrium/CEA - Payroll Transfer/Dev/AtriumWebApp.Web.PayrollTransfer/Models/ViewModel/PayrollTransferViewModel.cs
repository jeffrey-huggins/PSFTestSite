using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.PayrollTransfer.Models.ViewModel
{
    public class PayrollTransferViewModel
    {
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


        public IList<Employee> Employees { get; set; }
        public Employee Employee { get; set; }
        public IList<GeneralLedgerAccount> GLAccounts { get; set; }
        public GeneralLedgerAccount SourceGLAccount { get; set; }
        public GeneralLedgerAccount DestinationGLAccount { get; set; }
        public string EmployeeName { get { return (Employee != null ? Employee.LastName + ", " + Employee.FirstName : string.Empty); } }


        public IList<Community> Communities { get; set; }
        public Community SourceCommunity { get; set; }
        public Community DestinationCommunity { get; set; }
        

        public bool IsAdministrator { get; set; }
        public string AppCode { get; set; }
    }
}
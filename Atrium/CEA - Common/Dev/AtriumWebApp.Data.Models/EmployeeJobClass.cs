using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class EmployeeJobClass
    {
        [Key]
        public int EmployeeJobClassId { get; set; }
        public int EmployeeId { get; set; }
        public int JobClassId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StopDate { get; set; }
        public bool PrimaryFlg { get; set; }
        public int? JobClassGeneralLedgerId { get; set; }
        [JsonIgnore]
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
        [JsonIgnore]
        [ForeignKey("JobClassId")]
        public MasterJobClass JobClass { get; set; }
        [JsonIgnore]
        [ForeignKey("JobClassGeneralLedgerId")]
        public GeneralLedgerAccount GLAccount { get; set; }

    }
}

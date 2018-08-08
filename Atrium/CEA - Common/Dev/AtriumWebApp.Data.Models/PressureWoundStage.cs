using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class PressureWoundStage
    {
        [Key]
        public int PressureWoundStageId { get; set; }
        [Required]
        [MaxLength(20)]
        public string PressureWoundStageName { get; set; }
        public bool DataEntryFlg { get; set; }
        public bool ReportFlg { get; set; }
        public int SortOrder { get; set; }
        public bool IncludeInThresholdFlg { get; set; }
        public int? SeverityLevelNbr { get; set; }
        public bool LengthFlg {get;set;}
        public bool WidthFlg {get;set;}
        public bool DepthFlg { get; set; }

    }
}

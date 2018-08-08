using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SOCEventWound : SOCEvent
    {
        [Required]
        [StringLength(32)]
        public string Site { get; set; }
        public int? CompositeWoundDescribeId { get; set; }
        public bool UnavoidableFlg { get; set; }
        public bool AffectedByDiabetes { get; set; }
        public bool AffectedByIncontinence { get; set; }
        public bool AffectedByParalysis { get; set; }
        public bool AffectedBySepsis { get; set; }
        public bool AffectedByPeripheralVascularDisease { get; set; }
        public bool AffectedByEndStageDisease { get; set; }
        public bool AdmittedWithFlg { get; set; }
        public bool AffectedByOther { get; set; }
        [StringLength(64)]
        public string AffectedByOtherDescription { get; set; }
    }
}

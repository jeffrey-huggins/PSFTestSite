﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SOCFallTreatment
    {
        [Key]
        public int SOCFallTreatmentId { get; set; }
        [Required]
        [MaxLength(30)]
        public string SOCFallTreatmentName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
        public bool IncludeInThresholdFlg { get; set; }
    }
}
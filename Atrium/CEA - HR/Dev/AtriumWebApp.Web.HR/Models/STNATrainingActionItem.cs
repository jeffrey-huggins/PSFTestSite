using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.HR.Models
{
    public class STNATrainingActionItem
    {
        public int STNATrainingActionItemId { get; set; }
        
        [StringLength(64)]
        public string ActionItemDesc { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
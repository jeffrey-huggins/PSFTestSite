using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.HR.Models
{
    public class STNATrainingFacilityInteraction
    {
        [Key]
        public int STNATrainingFacilityInteractionId { get; set; }
        public int STNATrainingFacilityId { get; set; }
        public int STNATrainingActionItemId { get; set; }
        public DateTime InteractionDate { get; set; }
        //public DateTime InsertedDate { get; set; }
        //public DateTime? LastModifiedDate { get; set; }
    }
}
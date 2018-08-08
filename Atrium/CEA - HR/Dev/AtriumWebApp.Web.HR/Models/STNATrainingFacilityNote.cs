using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.HR.Models
{
    public class STNATrainingFacilityNote
    {
        [Key]
        public int STNATrainingFacilityNoteId { get; set; }
        public int STNATrainingFacilityId { get; set; }
        public DateTime NoteDate { get; set; }
        public string Note { get; set; }
        
        public DateTime InsertedDate { get; set; }
    }
}
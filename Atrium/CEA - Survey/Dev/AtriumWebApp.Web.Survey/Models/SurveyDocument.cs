using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Survey.Models
{
    public class SurveyDocument
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string SurveyCycleId { get; set; }
        [Required]
        public int SurveyId { get; set; }

        [MaxLength(256)]
        public string FileName { get; set; }
        [MaxLength(256)]
        public string ContentType { get; set; }
        public byte[] Document { get; set; }

        //public bool ArchiveFlg { get; set; }
        //public DateTime? ArchivedDate { get; set; }

        //public Survey Contract { get; set; }
    }
}
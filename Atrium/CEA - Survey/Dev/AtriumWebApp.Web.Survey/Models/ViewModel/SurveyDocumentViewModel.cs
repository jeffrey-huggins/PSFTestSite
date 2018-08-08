using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class SurveyDocumentViewModel
    {
        public int Id { get; set; }
        public string SurveyCycleId { get; set; }
        public int SurveyId { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }
        public string ContentType { get; set; }

        [DisplayName("File Upload")]
        public IFormFile Document { get; set; }

        //public bool ArchiveFlg { get; set; }
        //public DateTime? ArchivedDate { get; set; }

    }
}
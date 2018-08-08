using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace AtriumWebApp.Models.ViewModel
{
    public class ContractDocumentViewModel
    {
        public int Id { get; set; }
        public int ContractId { get; set; }

        [DisplayName("File Upload")]
        public IFormFile Document { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public bool ArchiveFlg { get; set; }
        public DateTime? ArchivedDate { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class PurchaseOrderDocumentViewModel
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }
        
        public string ContentType { get; set; }

        [DisplayName("File Upload")]
        public IEnumerable<IFormFile> Document { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Financial.Models
{
    public class PurchaseOrderDocument
    {
        [Key]
        public int Id { get; set; }

        //public int PurchaseOrderId { get; set; }
        //public PurchaseOrder PurchaseOrder { get; set; }

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Document { get; set; }
    }
}
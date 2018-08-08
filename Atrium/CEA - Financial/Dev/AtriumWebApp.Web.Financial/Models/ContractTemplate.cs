using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ContractTemplate : ContractBase
    {        
        [Required]
        [StringLength(128)]
        public string Description { get; set; }

        public DateTime SavedDate { get; set; }

        public byte[] Document { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

    }
}
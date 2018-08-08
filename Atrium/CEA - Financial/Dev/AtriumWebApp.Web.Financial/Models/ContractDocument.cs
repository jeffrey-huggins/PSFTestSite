using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ContractDocument
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ContractId { get; set; }
        public byte[] Document { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public bool ArchiveFlg { get; set; }
        public DateTime? ArchivedDate { get; set; }
        
        [ForeignKey("ContractId")]
        public virtual Contract Contract { get; set; }
    }
}
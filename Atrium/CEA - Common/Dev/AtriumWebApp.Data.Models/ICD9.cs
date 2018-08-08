using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class ICD9
    {
        [Key]
        public int ICD9Id { get; set; }
        public string SrcSystemICD9Id { get; set; }
        public string SrcSystemName { get; set; }
        public string IDC9Code { get; set; }
        public string DiseaseName { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
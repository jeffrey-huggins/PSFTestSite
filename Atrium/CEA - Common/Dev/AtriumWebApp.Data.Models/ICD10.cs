using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class ICD10
    {
        [Key]
        public int ICD10Id { get; set; }
        public string SrcSystemICD10Id { get; set; }
        public string SrcSystemName { get; set; }
        public string IDC10Code { get; set; }
        public string DiseaseName { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
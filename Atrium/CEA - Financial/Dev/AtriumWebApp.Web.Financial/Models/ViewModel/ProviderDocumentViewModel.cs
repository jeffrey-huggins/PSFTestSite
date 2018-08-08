using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace AtriumWebApp.Models.ViewModel
{
    public class ProviderDocumentViewModel
    {
        public int Id { get; set; }
        public int ContractProviderId { get; set; }

        [Required]
        [StringLength(128)]
        public string Description { get; set; }
        
        [Required]
        public DateTime SavedDate { get; set; }

        public IFormFile Document { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
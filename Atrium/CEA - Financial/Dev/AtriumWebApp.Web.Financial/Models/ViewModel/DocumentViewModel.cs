using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace AtriumWebApp.Models.ViewModel
{
    public class DocumentViewModel
    {
        public int Id { get; set; }

        [StringLength(128)]
        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
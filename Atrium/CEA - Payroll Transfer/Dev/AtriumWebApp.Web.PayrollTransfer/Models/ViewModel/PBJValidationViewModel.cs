using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.PayrollTransfer.Models.ViewModel
{
    public class PBJValidationViewModel
    {
        public DateTime QuarterStart { get; set; }
        public DateTime QuarterEnd { get; set; }
        public IFormFile PBJFile { get; set; }
        public IFormFile LookupFile { get; set; }

        public List<string> ErrorList { get; set; }
    }
}

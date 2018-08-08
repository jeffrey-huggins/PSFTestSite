using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using AtriumWebApp.Models.Structs;

namespace AtriumWebApp.Models.ViewModel
{
    public class ASAPHotlineViewModel
    {
        public List<ASAPCall> ASAPCalls { get; set; }
        public List<SelectListItem> Contacts { get; set; }
        public List<ASAPCallDocument> Documents { get; set; }
    }
}

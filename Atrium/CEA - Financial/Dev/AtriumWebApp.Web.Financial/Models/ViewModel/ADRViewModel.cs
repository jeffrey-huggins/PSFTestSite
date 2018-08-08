using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class ADRViewModel
    {
        public AdditionalDevelopmentRequest ADR { get; set; }
        public List<AdditionalDevelopmentRequest> ADRForCommunity { get; set; }
        public bool CanDelete { get; set; }
        public bool CanRequestNotes { get; set; }
    }
}

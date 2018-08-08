using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ProviderDocument : ContractBase
    {
        public int ContractProviderId { get; set; }
        public ContractProvider Provider { get; set; }

        public string Description { get; set; }

        public DateTime SavedDate { get; set; }

        public byte[] Document { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}

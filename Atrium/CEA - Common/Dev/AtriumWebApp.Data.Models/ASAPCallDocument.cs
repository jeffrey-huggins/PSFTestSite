using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ASAPCallDocument
    {
        public int ASAPCallDocumentId { get; set; }
        public int ASAPCallId { get; set; }
        public string DocumentFileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Document { get; set; }
    }
}
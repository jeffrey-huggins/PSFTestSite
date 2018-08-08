using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SOCPressureWoundDocument
    {
        [Key]
        public int SOCPressureWoundDocumentId { get; set; }
        public int SOCEventId { get; set; }
        public string ContentType { get; set; }
        public string DocumentFileName { get; set; }
        public byte[] Document { get; set; }

        public SOCEvent SOCEvent { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Financial.Models
{
    public class PurchaseOrderVendor
    {
        public int Id { get; set; }

        [Required]
        public int VendorClassId { get; set; }
        public PurchaseOrderVendorClass VendorClass { get; set; }

        [StringLength(20)]
        public string CorpVendorId { get; set; }

        [StringLength(64)]
        [Required]
        public string VendorName { get; set; }

        [StringLength(64)]
        [Required]
        public string Address1 { get; set; }

        [StringLength(64)]
        public string Address2 { get; set; }

        [StringLength(50)]
        [Required]
        public string City { get; set; }

        [StringLength(2)]
        [Required]
        public string StateCd { get; set; }

        [StringLength(20)]
        [Required]
        public string ZipCode { get; set; }

        [StringLength(16)]
        public string Phone { get; set; }

        [StringLength(16)]
        public string Fax { get; set; }

        [StringLength(128)]
        public string Email { get; set; }
        
        [Required]
        public bool AllowDataEntry { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public string FullAddress
        {
            get {
                var str = new System.Text.StringBuilder();
                str.AppendLine(Address1);
                if (!String.IsNullOrWhiteSpace(Address2))
                {
                    str.AppendLine(Address2);
                }
                str.Append(String.Format("{0}, {1} {2}", City, StateCd, ZipCode.Length > 5 ? ZipCode.Substring(0, 5) : ZipCode));

                return str.ToString();
            }
        }

        public string FullAddressRaw
        {
            get { return FullAddress.Replace("\r\n", "<br />"); }
        }
    }
}
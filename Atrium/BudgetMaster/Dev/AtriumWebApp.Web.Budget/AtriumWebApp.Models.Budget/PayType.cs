//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AtriumWebApp.Models.Budget
{
    using System;
    using System.Collections.Generic;
    
    public partial class PayType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PayType()
        {
            this.IntactAcctPayTypeMaps = new HashSet<IntactAcctPayTypeMap>();
        }
    
        public string PayTypeCd { get; set; }
        public string PayTypeNm { get; set; }
        public string PayGrpCd { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntactAcctPayTypeMap> IntactAcctPayTypeMaps { get; set; }
        public virtual PayGrp PayGrp { get; set; }
    }
}

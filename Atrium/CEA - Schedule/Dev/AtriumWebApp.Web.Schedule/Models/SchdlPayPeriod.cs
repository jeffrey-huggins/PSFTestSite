using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Schedule.Models
{
    public class SchdlPayPeriod
    {
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CommunityId { get; set; }
        [Required]
        [Column(TypeName ="date")]
        public DateTime PayPeriodBeginDate { get; set; }

        public virtual IList<SchdlPayerGroup> PayerGroups { get; set; }
        public virtual Community Community { get; set; }
    }
}
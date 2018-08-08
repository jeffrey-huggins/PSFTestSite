using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Objects.DataClasses;
using AtriumWebApp.Models;


namespace AtriumWebApp.Web.Schedule.Models
{
    public class TemplateSchdlPayPeriod
    {
        [EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CommunityId { get; set; }

        public virtual ICollection<TemplateSchdlPayerGroup> PayerGroups { get; set; }
        public virtual Community Community { get; set; }
    }
}
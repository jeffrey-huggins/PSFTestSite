using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class SkilledChartingGuideline
    {
        public SkilledChartingGuideline()
        {
            this.DocumentationQueues = new List<SkilledChartingDocumentationQueue>();
        }

        [Key]
        public int GuidelineId { get; set; }
        [Required]
        [MaxLength(256)]
        public string GuidelineName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public virtual List<SkilledChartingDocumentationQueue> DocumentationQueues { get; set; }
        public SkilledChartingDocumentationQueue NewDocQueue { get { return new SkilledChartingDocumentationQueue(); } }
    }
}
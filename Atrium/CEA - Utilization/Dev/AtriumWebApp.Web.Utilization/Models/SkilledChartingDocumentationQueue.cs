using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class SkilledChartingDocumentationQueue
    {
        [Key]
        public int DocumentationQueueId { get; set; }

        //[Key]
        public int GuidelineId { get; set; }
        //public virtual SkilledChartingGuideline Guideline { get; set; }
        [Required]
        [MaxLength(512)]
        public string DocumentationQueueName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
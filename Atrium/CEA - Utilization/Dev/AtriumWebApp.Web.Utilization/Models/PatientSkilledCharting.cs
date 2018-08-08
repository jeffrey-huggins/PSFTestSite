using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PatientSkilledCharting
    {
        [Key]
        public int PatientId { get; set; }

        [Key]
        public int DocumentationQueueId { get; set; }
        public SkilledChartingDocumentationQueue DocumentationQueue { get; set; }
    }
}
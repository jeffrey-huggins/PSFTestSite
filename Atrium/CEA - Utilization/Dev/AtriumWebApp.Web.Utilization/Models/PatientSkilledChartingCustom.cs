using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PatientSkilledChartingCustom
    {
        [Key]
        public int PatientId { get; set; }

        [Key]
        public int CustomQueueId { get; set; }

        public string CustomQueueText { get; set; }
    }
}
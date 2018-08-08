using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Survey.Models.ViewModel
{
    public class InstructionsViewModel
    {
        public int Id { get; set; }
        [StringLength(4096)]
        public string Instructions { get; set; }
        public DeficiencyRecordType RecordType { get; set; }

        public enum DeficiencyRecordType
        {
            Unknown = 0,
            Federal,
            Safety
        }
    }
}
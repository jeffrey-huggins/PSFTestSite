using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class ASAPContact
    {
        [Key]
        public int ASAPContactId { get; set; }
        [MaxLength(64)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(64)]
        [Required]
        public string LastName { get; set; }
        [EmailAddress]
        [Required]
        public string eMail { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class ASAPComplaintType
    {
        [Key]
        public int ASAPComplaintTypeId { get; set; }
        [MaxLength(32)]
        [Required]
        public string ASAPComplaintTypeDesc { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
    public class ASAPComplaintTypeContext : SharedContext
    {
        public ASAPComplaintTypeContext() : base(connectionString) { }
        public DbSet<ASAPComplaintType> AsapComplaintTypes { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ASAPComplaintType>().ToTable("MasterASAPComplaintType");
        }
    }
}

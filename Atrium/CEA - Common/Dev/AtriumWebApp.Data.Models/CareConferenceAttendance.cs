using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class CareConferenceAttendance
    {
        [Key]
        public int CareConferenceAttendanceId { get; set; }
        public int PatientId { get; set; }
        public DateTime CareConferenceDate { get; set; }
        public int? EmployeeId { get; set; }
    }

    public class CareConferenceAttendanceContext : DbContext
    {
        public CareConferenceAttendanceContext() : base("database") { }
        public DbSet<CareConferenceAttendance> Attendances { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CareConferenceAttendance>().ToTable("CareConferenceAttendance");
        }
    }
}

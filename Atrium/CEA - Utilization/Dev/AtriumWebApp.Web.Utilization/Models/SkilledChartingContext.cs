using System;
using System.Collections.Generic;
using System.Data.Entity;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class SkilledChartingContext : SharedContext
    {
        public SkilledChartingContext(string connString) : base(connString)
        {
            connectionString = connString;
        }
        public DbSet<SkilledChartingGuideline> SkilledChartingGuidelines { get; set; }
        public DbSet<SkilledChartingDocumentationQueue> SkilledChartingDocumentationQueues { get; set; }
        public DbSet<PatientSkilledCharting> PatientSkilledChartings { get; set; }
        public DbSet<PatientSkilledChartingCustom> PatientSkilledChartingCustoms { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SkilledChartingGuideline>().ToTable("MasterSkilledChartingGuideline");
            modelBuilder.Entity<SkilledChartingGuideline>().Property(p => p.GuidelineId).HasColumnName("SkilledChartingGuidelineId");
            modelBuilder.Entity<SkilledChartingGuideline>().Property(p => p.GuidelineName).HasColumnName("SkilledChartingGuidelineName");
            modelBuilder.Entity<SkilledChartingGuideline>().HasMany(p => p.DocumentationQueues); 

            modelBuilder.Entity<SkilledChartingDocumentationQueue>().ToTable("MasterSkilledChartingDocumentationQueue");
            modelBuilder.Entity<SkilledChartingDocumentationQueue>().Property(p => p.DocumentationQueueId).HasColumnName("SkilledChartingDocumentationQueueId").HasColumnOrder(0);
            modelBuilder.Entity<SkilledChartingDocumentationQueue>().Property(p => p.GuidelineId).HasColumnName("SkilledChartingGuidelineId").HasColumnOrder(1);
            modelBuilder.Entity<SkilledChartingDocumentationQueue>().Property(p => p.DocumentationQueueName).HasColumnName("SkilledChartingDocumentationQueueName");
            //modelBuilder.Entity<SkilledChartingDocumentationQueue>().HasRequired(p => p.Guideline).WithMany(p => p.DocumentationQueues).HasForeignKey(p => p.GuidelineId);

            modelBuilder.Entity<PatientSkilledCharting>().ToTable("PatientSkilledCharting");
            modelBuilder.Entity<PatientSkilledCharting>().Property(p => p.PatientId).HasColumnOrder(0);
            modelBuilder.Entity<PatientSkilledCharting>().Property(p => p.DocumentationQueueId).HasColumnName("SkilledChartingDocumentationQueueId").HasColumnOrder(1);

            modelBuilder.Entity<PatientSkilledChartingCustom>().ToTable("PatientSkilledChartingCustom");
            modelBuilder.Entity<PatientSkilledChartingCustom>().Property(p => p.PatientId).HasColumnOrder(0);
            modelBuilder.Entity<PatientSkilledChartingCustom>().Property(p => p.CustomQueueId).HasColumnName("SkilledChartingCustomQueueId").HasColumnOrder(1);
        }
    }
}
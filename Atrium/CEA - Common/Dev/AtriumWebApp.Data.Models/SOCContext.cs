using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    public class SOCContext : SharedContext
    {
        public SOCContext()
        {

        }

        public SOCContext(string connString) : base(connString)
        {
            connectionString = connString;
        }
        public DbSet<SOCEvent> SOCEvents { get; set; }
        public DbSet<SOCEventFallInjuryType> EventFallInjuryTypes { get; set; }
        public DbSet<SOCEventFallTreatment> FallTreatments { get; set; }
        public DbSet<SOCEventFallIntervention> FallInterventions { get; set; }
        public DbSet<SOCEventFallType> EventFallTypes { get; set; }
        public DbSet<SOCFallInjuryType> FallInjuryTypes { get; set; }
        public DbSet<SOCFallIntervention> FallInterventionTypes { get; set; }
        public DbSet<SOCFallLocation> FallLocations { get; set; }
        public DbSet<SOCFallTreatment> FallTreatmentTypes { get; set; }
        public DbSet<SOCFallType> FallTypes { get; set; }
        public DbSet<SOCCatheterType> CatheterTypes { get; set; }
        public DbSet<SOCRestraint> Restraints { get; set; }
        public DbSet<SOCEventRestraintNoted> EventRestraintNotes { get; set; }
        public DbSet<SOCAntiPsychoticMedication> AntiPsychoticMedications { get; set; }
        public DbSet<SOCAntiPsychoticDiagnosis> AntiPsychoticDiagnoses { get; set; }
        public DbSet<SOCEventAntiPsychoticNoted> AntiPsychoticNoted { get; set; }
        public DbSet<SOCEventWoundNoted> SOCEventWoundNotes { get; set; }
        public DbSet<Measure> MasterSOCMeasure { get; set; }
        public DbSet<PressureWoundStage> PressureWoundStages { get; set; }
        public DbSet<SOCPressureWoundDocument> PressureWoundDocuments { get; set; }
        public DbSet<CompositeWoundDescribe> CompositeWoundDescribes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SOCEvent>().ToTable("SOCEvent");
            modelBuilder.Entity<SOCEventFall>().ToTable("SOCEventFall");
            modelBuilder.Entity<SOCEventFallInjuryType>().ToTable("SOCEventFallInjuryType");
            modelBuilder.Entity<SOCEventFallInjuryType>().Property(f => f.SOCEventId).HasColumnOrder(0);
            modelBuilder.Entity<SOCEventFallInjuryType>().Property(f => f.SOCFallInjuryTypeId).HasColumnOrder(1);
            modelBuilder.Entity<SOCEventFallTreatment>().ToTable("SOCEventFallTreatment");
            modelBuilder.Entity<SOCEventFallTreatment>().Property(f => f.SOCEventId).HasColumnOrder(0);
            modelBuilder.Entity<SOCEventFallTreatment>().Property(f => f.SOCFallTreatmentId).HasColumnOrder(1);
            modelBuilder.Entity<SOCEventFallIntervention>().ToTable("SOCEventFallIntervention");
            modelBuilder.Entity<SOCEventFallIntervention>().Property(f => f.SOCEventId).HasColumnOrder(0);
            modelBuilder.Entity<SOCEventFallIntervention>().Property(f => f.SOCFallInterventionId).HasColumnOrder(1);
            modelBuilder.Entity<SOCEventFallType>().ToTable("SOCEventFallType");
            modelBuilder.Entity<SOCEventFallType>().Property(f => f.SOCEventId).HasColumnOrder(0);
            modelBuilder.Entity<SOCEventFallType>().Property(f => f.SOCFallTypeId).HasColumnOrder(1);
            modelBuilder.Entity<SOCFallInjuryType>().ToTable("MasterSOCFallInjuryType");
            modelBuilder.Entity<SOCFallIntervention>().ToTable("MasterSOCFallIntervention");
            modelBuilder.Entity<SOCFallLocation>().ToTable("MasterSOCFallLocation");
            modelBuilder.Entity<SOCFallTreatment>().ToTable("MasterSOCFallTreatment");
            modelBuilder.Entity<SOCFallType>().ToTable("MasterSOCFallType");
            modelBuilder.Entity<SOCCatheterType>().ToTable("MasterSOCCatheterType");
            modelBuilder.Entity<SOCEventCatheter>().ToTable("SOCEventCatheter");
            modelBuilder.Entity<SOCRestraint>().ToTable("MasterSOCRestraint");
            modelBuilder.Entity<SOCEventRestraintNoted>().ToTable("SOCEventRestraintNoted");
            modelBuilder.Entity<SOCEventAntiPsychotic>().ToTable("SOCEventAntiPsychotic");
            modelBuilder.Entity<SOCAntiPsychoticDiagnosis>().ToTable("MasterSOCAntiPsychoticDiagnosis");
            modelBuilder.Entity<SOCAntiPsychoticMedication>().ToTable("MasterSOCAntiPsychoticMedication");
            modelBuilder.Entity<SOCEventAntiPsychoticNoted>().ToTable("SOCEventAntiPsychoticNoted");
            modelBuilder.Entity<SOCEventWound>().ToTable("SOCEventWound");
            modelBuilder.Entity<SOCEventWound>().Property(w => w.AffectedByDiabetes).HasColumnName("PWDiabetesFlg");
            modelBuilder.Entity<SOCEventWound>().Property(w => w.AffectedByIncontinence).HasColumnName("PWIncontinenceFlg");
            modelBuilder.Entity<SOCEventWound>().Property(w => w.AffectedByParalysis).HasColumnName("PWParalysisFlg");
            modelBuilder.Entity<SOCEventWound>().Property(w => w.AffectedBySepsis).HasColumnName("PWSepsisFlg");
            modelBuilder.Entity<SOCEventWound>().Property(w => w.AffectedByPeripheralVascularDisease).HasColumnName("PWPeripheralVascularDiseaseFlg");
            modelBuilder.Entity<SOCEventWound>().Property(w => w.AffectedByEndStageDisease).HasColumnName("PWEndStageDiseaseFlg");
            modelBuilder.Entity<SOCEventWound>().Property(w => w.AffectedByOther).HasColumnName("PWOtherFlg");
            modelBuilder.Entity<SOCEventWound>().Property(w => w.AffectedByOtherDescription).HasColumnName("OtherDesc");
            modelBuilder.Entity<SOCEventWound>().Property(w => w.AdmittedWithFlg).HasColumnName("PWAdmittedWithFlg");
            modelBuilder.Entity<SOCEventWoundNoted>().ToTable("SOCEventWoundNoted");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.IdealBodyWeight).HasColumnName("PWIdealBodyWeight");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.ActualWeight).HasColumnName("PWActualWeight");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.FoodIntake).HasColumnName("PWFoodIntake");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.SkinTurgor).HasColumnName("PWSkinTurgor");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.Urine).HasColumnName("PWUrine");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.PainLevel).HasColumnName("PWPainLevel");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.DietaryDate).HasColumnName("PWDietaryDate").HasColumnType("date");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.PhysicianDate).HasColumnName("PWPhysicianDate").HasColumnType("date");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.FamilyDate).HasColumnName("PWFamilyDate").HasColumnType("date");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.Progress).HasColumnName("SCWProgress");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.AdmittedWithFlg).HasColumnName("PWAdmittedWithFlg");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.LengthNbr).HasColumnName("PWLengthNbr");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.WidthNbr).HasColumnName("PWWidthNbr");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.DepthNbr).HasColumnName("PWDepthNbr");
            modelBuilder.Entity<SOCEventWoundNoted>().Property(w => w.Status).HasColumnName("PWStatus");
            modelBuilder.Entity<SOCPressureWoundDocument>().ToTable("SOCPressureWoundDocument");
            modelBuilder.Entity<SOCPressureWoundDocument>().Property(d => d.ContentType).HasColumnName("ContentType");
            modelBuilder.Entity<SOCPressureWoundDocument>().Property(d => d.DocumentFileName).HasColumnName("DocumentFileName");
            modelBuilder.Entity<SOCPressureWoundDocument>().Property(d => d.Document).HasColumnName("Document");
            modelBuilder.Entity<Measure>().ToTable("MasterSOCMeasure");
            modelBuilder.Entity<PressureWoundStage>().ToTable("MasterPressureWoundStage");
            modelBuilder.Entity<CompositeWoundDescribe>().ToTable("MasterCompositeWoundDescribe");
        }
    }
}

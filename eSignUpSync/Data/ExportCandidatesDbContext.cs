using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSignUpSync.Data
{
    public class ExportCandidatesDbContext : DbContext
    {
        public ExportCandidatesDbContext(DbContextOptions<ExportCandidatesDbContext> options)
            : base(options) { }

        //ExportCandidates (EC)
        public DbSet<Models.ExportCandidates.ApprenticeshipEmployerModel> ApprenticeshipEmployer { get; set; }
        public DbSet<Models.ExportCandidates.CandidateModel> Candidate { get; set; }
        public DbSet<Models.ExportCandidates.CandidateDocumentModel> CandidateDocument { get; set; }
        public DbSet<Models.ExportCandidates.CandidateExtraFieldsModel> CandidateExtraFields { get; set; }
        public DbSet<Models.ExportCandidates.CandidateNoteModel> CandidateNote { get; set; }
        public DbSet<Models.ExportCandidates.CandidateQualificationModel> CandidateQualification { get; set; }
        public DbSet<Models.ExportCandidates.ContactPreferenceModel> ContactPreference { get; set; }
        public DbSet<Models.ExportCandidates.CustomFieldModel> CustomField { get; set; }
        public DbSet<Models.ExportCandidates.EmploymentStatusMonitoringModel> EmploymentStatusMonitoring { get; set; }
        public DbSet<Models.ExportCandidates.EnglishAndMathsQualificationModel> EnglishAndMathsQualification { get; set; }
        public DbSet<Models.ExportCandidates.EnglishMathsComponentModel> EnglishMathsComponent { get; set; }
        public DbSet<Models.ExportCandidates.HouseholdSituationModel> HouseholdSituation { get; set; }
        public DbSet<Models.ExportCandidates.LearnerEmploymentStatusModel> LearnerEmploymentStatus { get; set; }
        public DbSet<Models.ExportCandidates.LLDDAndHealthProblemModel> LLDDAndHealthProblem { get; set; }
        public DbSet<Models.ExportCandidates.LLDDAndHealthProblemPeopleSoftModel> LLDDAndHealthProblemPeopleSoft { get; set; }
        public DbSet<Models.ExportCandidates.OnboardingDocumentModel> OnboardingDocument { get; set; }
        public DbSet<Models.ExportCandidates.PlacedRecruitmentModel> PlacedRecruitment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Needed to avoid error:
            //The dependent side could not be determined for the one-to-one relationship between 'ApprenticeshipEmployerModel.PlacedRecruitment' and 'PlacedRecruitmentModel.ApprenticeshipEmployer'.
            //To identify the dependent side of the relationship, configure the foreign key property.
            //If these navigations should not be part of the same relationship, configure them independently via separate method chains in 'OnModelCreating'.
            modelBuilder.Entity<Models.ExportCandidates.PlacedRecruitmentModel>()
            .HasOne(a => a.ApprenticeshipEmployer)
            .WithOne(a => a.PlacedRecruitment)
            .HasForeignKey<Models.ExportCandidates.ApprenticeshipEmployerModel>(c => c.VacancyID);
        }
    }
}

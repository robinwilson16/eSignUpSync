using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSignUpSync.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        //Candidates
        public DbSet<Models.Candidates.CandidateModel> Candidate { get; set; }
        public DbSet<Models.Candidates.CandidateDisabilityLearningDifficultyResultModel> CandidateDisabilityLearningDifficultyResult { get; set; }
        public DbSet<Models.Candidates.CandidateDocumentModel> CandidateDocument { get; set; }
        public DbSet<Models.Candidates.CandidateExtraFieldModel> CandidateExtraField { get; set; }
        public DbSet<Models.Candidates.CandidateNoteModel> CandidateNote { get; set; }
        public DbSet<Models.Candidates.CandidateQualificationModel> CandidateQualification { get; set; }
        public DbSet<Models.Candidates.CustomFieldValueModel> CustomFieldValue { get; set; }

        //ExportCandidates (EC)
        //public DbSet<Models.ExportCandidates.ApprenticeshipEmployerModel> ECApprenticeshipEmployer { get; set; }
        //public DbSet<Models.ExportCandidates.CandidateModel> ECCandidate { get; set; }
        //public DbSet<Models.ExportCandidates.CandidateDocumentModel> ECCandidateDocument { get; set; }
        //public DbSet<Models.ExportCandidates.CandidateExtraFieldsModel> ECCandidateExtraFields { get; set; }
        //public DbSet<Models.ExportCandidates.CandidateNoteModel> ECCandidateNote { get; set; }
        //public DbSet<Models.ExportCandidates.CandidateQualificationModel> ECCandidateQualification { get; set; }
        //public DbSet<Models.ExportCandidates.ContactPreferenceModel> ECContactPreference { get; set; }
        //public DbSet<Models.ExportCandidates.CustomFieldModel> ECCustomField { get; set; }
        //public DbSet<Models.ExportCandidates.EmploymentStatusMonitoringModel> ECEmploymentStatusMonitoring { get; set; }
        //public DbSet<Models.ExportCandidates.EnglishAndMathsQualificationModel> ECEnglishAndMathsQualification { get; set; }
        //public DbSet<Models.ExportCandidates.EnglishMathsComponentModel> ECEnglishMathsComponent { get; set; }
        //public DbSet<Models.ExportCandidates.HouseholdSituationModel> ECHouseholdSituation { get; set; }
        //public DbSet<Models.ExportCandidates.LearnerEmploymentStatusModel> ECLearnerEmploymentStatus { get; set; }
        //public DbSet<Models.ExportCandidates.LLDDAndHealthProblemModel> ECLLDDAndHealthProblem { get; set; }
        //public DbSet<Models.ExportCandidates.LLDDAndHealthProblemPeopleSoftModel> ECLLDDAndHealthProblemPeopleSoft { get; set; }
        //public DbSet<Models.ExportCandidates.OnboardingDocumentModel> ECOnboardingDocument { get; set; }
        //public DbSet<Models.ExportCandidates.PlacedRecruitmentModel> ECPlacedRecruitment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Needed to avoid error:
            //The dependent side could not be determined for the one-to-one relationship between 'ApprenticeshipEmployerModel.PlacedRecruitment' and 'PlacedRecruitmentModel.ApprenticeshipEmployer'.
            //To identify the dependent side of the relationship, configure the foreign key property.
            //If these navigations should not be part of the same relationship, configure them independently via separate method chains in 'OnModelCreating'.
            //modelBuilder.Entity<Models.ExportCandidates.PlacedRecruitmentModel>()
            //.HasOne(a => a.ApprenticeshipEmployer)
            //.WithOne(a => a.PlacedRecruitment)
            //.HasForeignKey<Models.ExportCandidates.ApprenticeshipEmployerModel>(c => c.VacancyID);
        }
    }
}

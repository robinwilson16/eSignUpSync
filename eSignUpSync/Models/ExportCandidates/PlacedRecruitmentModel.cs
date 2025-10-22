﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eSignUpSync.Models.ExportCandidates
{
    public class PlacedRecruitmentModel
    {
        [JsonIgnore]
        public int ID { get; set; }

        public string? VacancyID { get; set; }

        [JsonPropertyName("minOTJTraining")]
        public string? MinOTJTraining { get; set; }

        [JsonIgnore]
        [Display(Name = "Min OTJ Training")]
        public decimal? minOTJTraining
        {
            get { return MinOTJTraining == null ? null : decimal.Parse(MinOTJTraining ?? "", new CultureInfo("en-GB")); }
        }

        public string? RecruitmentStatus { get; set; }
        public string? ProgrammeGroupCode { get; set; }
        public ICollection<string>? ProgrammeSites { get; set; }
        public string? ProgrammeCode { get; set; }
        public string? VacancyType { get; set; }

        [JsonPropertyName("actualStartDate")]
        public string? ActualStartDate { get; set; }

        [JsonIgnore]
        [Display(Name = "Actual Start Date")]
        public DateTime? actualStartDate
        {
            get { return ActualStartDate == null ? null : DateTime.ParseExact(ActualStartDate ?? "", "yyyy-MM-dd", new CultureInfo("en-GB")); }
        }

        [JsonPropertyName("signedUpDate")]
        public string? SignedUpDate { get; set; }

        [JsonIgnore]
        [Display(Name = "Signed Up Date")]
        public DateTime? signedUpDate
        {
            get { return SignedUpDate == null ? null : DateTime.ParseExact(SignedUpDate ?? "", "yyyy-MM-dd", new CultureInfo("en-GB")); }
        }

        [JsonPropertyName("endDate")]
        public string? EndDate { get; set; }

        [JsonIgnore]
        [Display(Name = "End Date")]
        public DateTime? endDate
        {
            get { return EndDate == null ? null : DateTime.ParseExact(EndDate ?? "", "yyyy-MM-dd", new CultureInfo("en-GB")); }
        }

        [JsonPropertyName("trainingPlanOTJHours")]
        public string? TrainingPlanOTJHours { get; set; }

        [JsonIgnore]
        [Display(Name = "Training Plan OTJ Hours")]
        public decimal? trainingPlanOTJHours
        {
            get { return TrainingPlanOTJHours == null ? null : decimal.Parse(TrainingPlanOTJHours ?? "", new CultureInfo("en-GB")); }
        }

        public string? HoursPerWeek { get; set; }

        [JsonPropertyName("isPartTimeHours")]
        public string? IsPartTimeHours { get; set; }

        [JsonIgnore]
        [Display(Name = "Is Part Time Hours")]
        public bool? isPartTimeHours
        {
            get { return IsPartTimeHours == null ? null : IsPartTimeHours == "Yes" ? true : false; }
        }

        public string? DurationMonths { get; set; }
        public string? RPLReductionInWeeks { get; set; }
        public string? SubcontractorUKPRN { get; set; }
        public string? NegotiatedRate { get; set; }
        public string? EPACost { get; set; }
        public string? TNP1 { get; set; }
        public string? RPLReductionPrice { get; set; }
        public string? ApprenticeshipStandardCode { get; set; }
        public string? ApprenticeshipStandardTitle { get; set; }
        public string? ApprenticeshipStandardVersion { get; set; }
        public string? ApprenticeshipVacancyTitle { get; set; }
        public string? CohortRefNo { get; set; }

        [JsonPropertyName("fundingStartDate")]
        public string? FundingStartDate { get; set; }

        [JsonIgnore]
        [Display(Name = "Funding Start Date")]
        public DateTime? fundingStartDate
        {
            get { return FundingStartDate == null ? null : DateTime.ParseExact(FundingStartDate ?? "", "yyyy-MM-dd", new CultureInfo("en-GB")); }
        }

        public string? RecruitmentID { get; set; }
        public int? ProgressReviewFrequencyInWeeks { get; set; }
        public ICollection<string>? ProgressReviewEstimatedDates { get; set; }
        public ApprenticeshipEmployerModel? ApprenticeshipEmployer { get; set; }
        public ICollection<HouseholdSituationModel>? HouseholdSituations { get; set; }
        public ICollection<OnboardingDocumentModel>? OnboardingDocuments { get; set; }
        public ICollection<EnglishMathsComponentModel>? EnglishMathsComponents { get; set; }
        public string? EndpointAssessorName { get; set; }
        public string? EndpointAssessorReference { get; set; }
        public string? KeyAccountManager { get; set; }
        public string? LeadRecruitmentOfficer { get; set; }
        public string? Subcontractor { get; set; }
        public string? SubcontractorContact { get; set; }
        public string? SubcontractorContactEmail { get; set; }
        public string? Employer { get; set; }
        public string? EmployerContact { get; set; }
        public string? EmployerContactEmail { get; set; }
        public string? AcademicLevel { get; set; }
        public bool? IsEnrolled { get; set; }
        public decimal? TotalRPLOTJHours { get; set; }
        public decimal? PlannedSkillScanHours { get; set; }

        [JsonIgnore]
        public CandidateModel? Candidate { get; set; }
    }
}

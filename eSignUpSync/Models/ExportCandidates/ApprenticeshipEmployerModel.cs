﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace eSignUpSync.Models.ExportCandidates
{
    public class ApprenticeshipEmployerModel
    {
        [JsonIgnore]
        public int ID { get; set; }

        [JsonIgnore]
        public int VacancyID { get; set; }

        [JsonPropertyName("employerID")]
        public string? EmployerID { get; set; }

        [JsonIgnore]
        [Display(Name = "Employer ID")]
        public int? employerID
        {
            get { return EmployerID == null ? null : int.Parse(EmployerID ?? "", new CultureInfo("en-GB")); }
        }

        public string? Name { get; set; }

        [JsonPropertyName("edrsNumber")]
        public string? EDRSNumber { get; set; }

        [JsonIgnore]
        [Display(Name = "EDRS Number")]
        public int? eDRSNumber
        {
            get {
                int.TryParse(EDRSNumber, new CultureInfo("en-GB"), out int EDRSNumberInt);
                return EDRSNumberInt; 
            }
        }

        public string? VacancyEmployerSiteName { get; set; }
        public string? VacancyEmployerSiteAddress1 { get; set; }
        public string? VacancyEmployerSiteAddress2 { get; set; }
        public string? VacancyEmployerSiteTown { get; set; }
        public string? VacancyEmployerSitePostCode { get; set; }

        [JsonIgnore]
        public PlacedRecruitmentModel? PlacedRecruitment { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eSignUpSync.Models.ExportCandidates
{
    public class ContactPreferenceModel
    {
        [Key]
        public string? ContactPreferenceID { get; set; }
        public string? ContPrefDesc { get; set; }
        public string? ContPrefType { get; set; }
        public string? ContPrefCode { get; set; }
        
        [JsonIgnore]
        public CandidateModel? Candidate { get; set; }
    }
}

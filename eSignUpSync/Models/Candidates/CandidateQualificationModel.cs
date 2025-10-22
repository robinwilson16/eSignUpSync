using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace eSignUpSync.Models.Candidates
{
    public class CandidateQualificationModel
    {
        [Key]
        public int? ID { get; set; }
        public string? QualificationTitle { get; set; }
        public string? QualificationReference { get; set; }
        public string? QualificationTypeName { get; set; }
        public string? OrganisationName { get; set; }
        public string? Grade { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfAward { get; set; }
        public bool? HighestAward { get; set; }

        [JsonIgnore]
        public int? CandidateID { get; set; }

        [JsonIgnore]
        public CandidateModel? Candidate { get; set; }
    }
}

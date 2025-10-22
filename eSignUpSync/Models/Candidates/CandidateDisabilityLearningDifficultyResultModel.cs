using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace eSignUpSync.Models.Candidates
{
    public class CandidateDisabilityLearningDifficultyResultModel
    {
        [Key]
        public int? ID { get; set; }

        [Required]
        public int? CandidateID { get; set; } //Include this ID field but not in the other related tables

        [JsonIgnore]
        public CandidateModel? Candidate { get; set; }
        public int? CandidateDisabilityLearningDifficultiesID { get; set; }
        public string? Name { get; set; }
    }
}

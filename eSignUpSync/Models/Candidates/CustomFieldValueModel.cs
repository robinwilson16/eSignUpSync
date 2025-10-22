using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace eSignUpSync.Models.Candidates
{
    [PrimaryKey(nameof(CandidateID), nameof(Label))]
    public class CustomFieldValueModel
    {
        public string? Value { get; set; }

        [Required]
        public string? Label { get; set; }

        [JsonIgnore]
        public int? CandidateID { get; set; }

        [JsonIgnore]
        public CandidateModel? Candidate { get; set; }
    }
}

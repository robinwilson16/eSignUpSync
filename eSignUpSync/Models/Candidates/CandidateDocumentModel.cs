using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace eSignUpSync.Models.Candidates
{
    public class CandidateDocumentModel
    {
        public string? QualificationType { get; set; }
        public string? FileName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastUpdatedDate { get; set; }
        public string? DocumentURL { get; set; }

        [Key]
        public int? ID { get; set; }

        [JsonIgnore]
        public int? CandidateID { get; set; }

        [JsonIgnore]
        public CandidateModel? Candidate { get; set; }
    }
}

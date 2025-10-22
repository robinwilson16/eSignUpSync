using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace eSignUpSync.Models.Candidates
{
    public class CandidateNoteModel
    {
        [Key]
        public int ID { get; set; }
        public string? Notes { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LastUpdatedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreatedOn { get; set; }
        public int? LastUpdatedBy { get; set; }

        public int? CreatedNoteUserID { get; set; }

        [JsonIgnore]
        public int? CandidateID { get; set; }

        [JsonIgnore]
        public CandidateModel? Candidate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eSignUpSync.Models.ExportCandidates
{
    public class LearnerEmploymentStatusModel
    {
        [JsonIgnore]
        public int ID { get; set; }

        public string? EmpStat { get; set; }
        public ICollection<EmploymentStatusMonitoringModel>? EmploymentStatusMonitoring { get; set; }

        [JsonIgnore]
        public CandidateModel? Candidate { get; set; }
    }
}

using eSignUpSync.Models.Candidates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eSignUpSync.Helpers
{
    public static class JsonOutput
    {
        public static void WriteCandidateJson(ILogger logger, Models.Candidates.CandidateModel candidate, bool indented = true)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = indented,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string json = JsonSerializer.Serialize(candidate, options);

            logger.LogInformation(json);
        }

        public static void WriteExportCandidatesJson(ILogger logger, List<Models.ExportCandidates.CandidateModel> candidates, bool indented = true)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = indented,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string json = JsonSerializer.Serialize(candidates, options);

            logger.LogInformation(json);
        }
    }
}

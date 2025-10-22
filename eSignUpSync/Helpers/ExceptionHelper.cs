using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eSignUpSync.Helpers
{
    public class ExceptionHelper
    {
        /// <summary>
        /// Create a readable, actionable message for endpoint-related exceptions.
        /// </summary>
        public static string FormatEndpointException(Exception ex, string? endpoint = null, bool includeStack = false)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(endpoint))
            {
                sb.AppendLine($"Endpoint: {endpoint}");
            }

            sb.AppendLine($"Timestamp (UTC): {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"ExceptionType: {ex.GetType().FullName}");
            sb.AppendLine($"Message: {ex.Message}");

            if (ex is HttpRequestException httpEx)
            {
                if (httpEx.StatusCode != null)
                {
                    sb.AppendLine($"StatusCode: {(int)httpEx.StatusCode} ({httpEx.StatusCode})");
                }
            }

            if (ex.InnerException != null)
            {
                sb.AppendLine("InnerException:");
                sb.AppendLine($"  Type: {ex.InnerException.GetType().FullName}");
                sb.AppendLine($"  Message: {ex.InnerException.Message}");
            }

            if (includeStack && ex.StackTrace != null)
            {
                sb.AppendLine("StackTrace:");
                sb.AppendLine(ex.StackTrace);
            }

            sb.AppendLine();
            sb.AppendLine("Action: Verify endpoint URL, credentials, network connectivity and API availability.");

            return sb.ToString();
        }
    }
}

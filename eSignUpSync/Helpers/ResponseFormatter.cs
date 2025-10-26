using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace eSignUpSync.Helpers
{
    public static class ResponseFormatter
    {
        /// <summary>
        /// Attempts to parse and pretty-print a JSON response body, and extracts a concise summary
        /// of common error fields (message, error, errors, detail, title, exceptionMessage).
        /// If the content is not JSON the raw (truncated) string is returned.
        /// </summary>
        public static string FormatResponseContent(string content, int maxLength = 4000)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "<empty response body>";

            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                // Attempt to extract a concise summary from common fields
                string? summary = TryExtractSummary(root);

                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(summary))
                {
                    sb.AppendLine("Summary:");
                    sb.AppendLine(summary);
                    sb.AppendLine();
                }

                var result = sb.ToString();
                return Truncate(result, maxLength);
            }
            catch (JsonException)
            {
                // Not JSON — return a truncated raw representation
                return $"Raw response: {Truncate(content, maxLength)}";
            }
            catch (Exception ex)
            {
                return $"Failed to format response content: {ex.Message}. Raw: {Truncate(content, maxLength)}";
            }
        }

        private static string? TryExtractSummary(JsonElement node)
        {
            string[] keys = new[] { "message", "error", "errors", "detail", "title", "exceptionMessage", "exception" };

            foreach (var key in keys)
            {
                if (TryFindProperty(node, key, out var found))
                {
                    switch (found.ValueKind)
                    {
                        case JsonValueKind.String:
                            return found.GetString();
                        case JsonValueKind.Object:
                        case JsonValueKind.Array:
                            try
                            {
                                // For arrays/objects serialize a compact representation (first element or object)
                                var compact = JsonSerializer.Serialize(found, new JsonSerializerOptions { WriteIndented = false });
                                return compact;
                            }
                            catch
                            {
                                return found.ToString();
                            }
                        default:
                            try
                            {
                                return found.ToString();
                            }
                            catch
                            {
                                return null;
                            }
                    }
                }
            }

            // No known keys — for shallow objects try "errors" property that may be an object with details
            return null;
        }

        private static bool TryFindProperty(JsonElement element, string targetKey, out JsonElement found)
        {
            // default out value
            found = default;

            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in element.EnumerateObject())
                {
                    if (string.Equals(prop.Name, targetKey, StringComparison.OrdinalIgnoreCase))
                    {
                        found = prop.Value;
                        return true;
                    }

                    if (TryFindProperty(prop.Value, targetKey, out found))
                        return true;
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    if (TryFindProperty(item, targetKey, out found))
                        return true;
                }
            }

            return false;
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            if (s.Length <= max) return s;
            return s.Substring(0, max) + "...[truncated]";
        }
    }
}
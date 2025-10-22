using eSignUpEBSAPI.Models;
using eSignUpSync.Helpers;
using eSignUpSync.Models;
using eSignUpSync.Models.Candidates;
using eSignUpSync.Models.ExportCandidates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace eSignUpSync.Services
{
    public class Export
    {
        //Get temporary API Token from eSignUp API using Client and Secret
        public static async Task<APIAccessToken> GetESignUpAPIToken(ILogger logger, HttpClient httpClient, SettingsESignUpModel settingsESignUp)
        {
            APIAccessToken? aPIAccessToken;

            string endpointLogin = $"Login/GetAccessToken?Client={settingsESignUp.Client}&Secret={settingsESignUp.Secret}";

            try
            {
                aPIAccessToken = await httpClient.GetFromJsonAsync<APIAccessToken>(endpointLogin);
            }
            catch (HttpRequestException e)
            {
                string msg = ExceptionHelper.FormatEndpointException(e, endpointLogin);
                logger.LogError(msg);
                return new APIAccessToken();
            }

            return aPIAccessToken ?? new();
        }

        public static async Task<List<Models.Candidates.CandidateModel>?> GetLocalCandidates(ILogger logger, HttpClient httpClient)
        {
            List<Models.Candidates.CandidateModel>? candidates;

            string endpointCandidates = $"Candidate";

            try
            {
                candidates = await httpClient.GetFromJsonAsync<List<Models.Candidates.CandidateModel>?>(endpointCandidates);
            }
            catch (HttpRequestException e)
            {
                string msg = ExceptionHelper.FormatEndpointException(e, endpointCandidates);
                logger.LogError(msg);
                return new List<Models.Candidates.CandidateModel>();
            }

            return candidates ?? new();
        }

        public static async Task<List<Models.Candidates.CandidateModel>?> GetESignUpCandidates(ILogger logger, HttpClient httpClient)
        {
            List<Models.Candidates.CandidateModel>? candidates = new();

            string endpointCandidates = $"Candidates/GetAll";
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            try
            {
                httpResponse = await httpClient.PostAsJsonAsync<Models.Candidates.CandidateModel?>(endpointCandidates, null);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();
                    logger.LogError($"Error loading candidates from eSignUp");
                }
                else
                {
                    candidates = await httpResponse.Content.ReadFromJsonAsync<List<Models.Candidates.CandidateModel>?>();
                }
            }
            catch (HttpRequestException e)
            {
                string msg = ExceptionHelper.FormatEndpointException(e, endpointCandidates);
                logger.LogError(msg);
                return new List<Models.Candidates.CandidateModel>();
            }

            return candidates ?? new();
        }

        public static async Task<int?> SendLocalCandidates(ILogger logger, HttpClient httpClient, List<Models.Candidates.CandidateModel>? candidates)
        {
            string endpointCandidates = $"Candidates/Add";
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            int totalRecords = candidates?.Count ?? 0;
            int? recordsSent = 0;
            int nextPercent = 10;
            int nextThreshold = (int)Math.Ceiling(totalRecords * nextPercent / 100.0);

            if (totalRecords == 0)
            {
                logger.LogInformation("No candidates to send.");
                return 0;
            }

            logger.LogInformation($"Sending {totalRecords} candidates to API...");

            foreach (var candidate in candidates ?? new())
            {
                //if (candidate != null)
                //{
                //    logger.LogInformation($"---------------------------");
                //    eSignUpSync.Helpers.JsonOutput.WriteCandidateJson(logger, candidate);
                //    logger.LogInformation($"---------------------------");
                //}

                try
                {
                    logger.LogInformation($"Sending candidate {candidate.Surname}, {candidate.FirstNames} ({candidate.StudentID})...");

                    candidate.ID = 0; // Ensure ID is 0 for new record creation
                    if (candidate.CandidateExtraFields != null)
                        candidate.CandidateExtraFields.ID = 0;

                    if (candidate?.CandidateDisabilityLearningDifficultyResults?.Count > 0)
                    {
                        foreach (var item in candidate.CandidateDisabilityLearningDifficultyResults)
                        {
                            item.ID = 0;
                        }
                    }

                    if (candidate?.CandidateDocuments?.Count > 0)
                    {
                        foreach (var item in candidate.CandidateDocuments)
                        {
                            item.ID = 0;
                        }
                    }

                    if (candidate?.CandidateNotes?.Count > 0)
                    {
                        foreach (var item in candidate.CandidateNotes)
                        {
                            item.ID = 0;
                        }
                    }

                    if (candidate?.CandidateQualifications?.Count > 0)
                    {
                        foreach (var item in candidate.CandidateQualifications)
                        {
                            item.ID = 0;
                        }
                    }

                    httpResponse = await httpClient.PostAsJsonAsync(endpointCandidates, candidate);
                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        string responseContent = await httpResponse.Content.ReadAsStringAsync();
                        logger.LogError($"Error sending candidate {candidate?.Surname}, {candidate?.FirstNames} ({candidate?.StudentID}). Status Code: {httpResponse.StatusCode}. Response: {responseContent}");
                    }
                    else {
                        recordsSent ++;

                        // Check and report progress for every 10% milestone reached
                        while (nextPercent <= 100 && recordsSent >= nextThreshold)
                        {
                            logger.LogInformation($"Progress: {recordsSent}/{totalRecords} ({nextPercent}%)");
                            nextPercent += 10;
                            nextThreshold = (int)Math.Ceiling(totalRecords * nextPercent / 100.0);
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    string msg = ExceptionHelper.FormatEndpointException(e, endpointCandidates);
                    logger.LogError(msg);
                    throw;
                }
            }

            // Ensure final completion log shows 100% if not already logged
            if (recordsSent < totalRecords)
            {
                logger.LogInformation($"Completed sending candidates: {recordsSent}/{totalRecords}");
            }

            return recordsSent;
        }
    }


}

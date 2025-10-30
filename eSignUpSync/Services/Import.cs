using eSignUpSync.Helpers;
using eSignUpSync.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace eSignUpSync.Services
{
    public class Import
    {
        public static List<Models.ExportCandidates.CandidateModel>? CandidatesLocal { get; set; }
        public static List<Models.ExportCandidates.CandidateModel>? CandidatesESignUp { get; set; }

        public static async Task<int?> DoImport(
            ILogger logger,
            HttpClient httpClientLocalData,
            HttpClient httpClientESignUp,
            APIAccessToken? apiAccessToken)
        {
            //Check API Access Token
            if (apiAccessToken == null || string.IsNullOrEmpty(apiAccessToken.Token))
            {
                logger.LogError("Error: eSignUp API Access Token is blank. Exiting.");
                return 1;
            }
            else
            {
                //Set Authorization Header for eSignUp HTTP Client    
                httpClientESignUp.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiAccessToken?.Token);
            }

            //Get eSignUp Export Candidates
            CandidatesESignUp = await GetESignUpExportCandidates(logger, httpClientESignUp);
            int? totalCandidates = CandidatesESignUp?.Count() ?? 0;

            logger.LogInformation($"\nLoaded Export Candidates from eSignUp: {totalCandidates}\n");
            if (CandidatesESignUp == null || CandidatesESignUp.Count() == 0)
            {
                logger.LogWarning("No export candidates found in eSignUp");
                //Not an error as may be no new candidates to import so return 0
                return 0;
            }

            //Save candidates to file for debugging
            //List<Models.ExportCandidates.CandidateModel>? savedToFileCandidates =
            //    await SaveLocalCandidatesToFile(logger, httpClientLocalData, CandidatesESignUp);

            //bool? candidatesDeleted = await DeleteLocalCandidatesFromDatabase(logger, httpClientLocalData);

            CandidatesLocal = await SaveLocalCandidatesToDatabase(logger, httpClientLocalData, CandidatesESignUp);
            if (CandidatesLocal == null || CandidatesLocal.Count() == 0)
            {
                logger.LogError("Error saving eSignUp Export Candidates to local MIS system.");
                return 1;
            }
            else if (CandidatesLocal.Count() < totalCandidates)
            {
                logger.LogWarning("Some candidates may not have been sent successfully. Please check the logs for details.");
                return 1;
            }

            logger.LogInformation($"\nTotal Export Candidates Saved to Local MIS System: {CandidatesLocal?.Count()}\n");

            return 0;
        }
        
        public static async Task<List<Models.ExportCandidates.CandidateModel>?> GetESignUpExportCandidates(ILogger logger, HttpClient httpClient)
        {
            List<Models.ExportCandidates.CandidateModel>? exportCandidates = new();

            string endpointExportCandidates = $"ExportCandidates";

            try
            {
                exportCandidates = await httpClient.GetFromJsonAsync<List<Models.ExportCandidates.CandidateModel>?>(endpointExportCandidates);
            }
            catch (HttpRequestException e)
            {
                string msg = ExceptionHelper.FormatEndpointException(e, endpointExportCandidates, httpClient);
                logger.LogError(msg);
                return new List<Models.ExportCandidates.CandidateModel>();
            }

            return exportCandidates ?? new();
        }

        public static async Task<List<Models.ExportCandidates.CandidateModel>?> GetLocalCandidatesFromDatabase(ILogger logger, HttpClient httpClient)
        {
            List<Models.ExportCandidates.CandidateModel>? localCandidates = new();
            string endpointExportCandidates = $"ExportCandidate";
            try
            {
                localCandidates = await httpClient.GetFromJsonAsync<List<Models.ExportCandidates.CandidateModel>?>(endpointExportCandidates);

                logger.LogInformation($"\nLoaded Export Candidates from local database: {localCandidates?.Count}\n");
            }

            catch (HttpRequestException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    logger.LogInformation($"No records found as may have already been deleted");
                    return new List<Models.ExportCandidates.CandidateModel>();
                }
                else
                {
                    string msg = ExceptionHelper.FormatEndpointException(e, endpointExportCandidates, httpClient);
                    logger.LogError(msg);
                    return new List<Models.ExportCandidates.CandidateModel>();
                }     
            }
            return localCandidates ?? new();
        }

        public static async Task<bool?> DeleteLocalCandidatesFromDatabase(ILogger logger, HttpClient httpClient)
        {
            List<Models.ExportCandidates.CandidateModel>? currentCandidates = new();

            currentCandidates = await GetLocalCandidatesFromDatabase(logger, httpClient);

            List<Models.ExportCandidates.CandidateModel>? deletedCandidates = new();

            if (currentCandidates == null || currentCandidates.Count() == 0)
            {
                logger.LogWarning("No candidates to delete locally.");

                //Not an error - nothing to delete
                return true;
            }

            string endpointExportCandidates = $"ExportCandidate/All/Y";
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            try
            {
                //Does not return candidates currently so will always be NULL
                logger.LogInformation($"Deleting {currentCandidates.Count()} existing candidates from local database.");
                httpResponse = await httpClient.DeleteAsync(endpointExportCandidates);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();
                    string responseFormatted = ResponseFormatter.FormatResponseContent(responseContent, 10000);

                    logger.LogError("Error saving candidates to local database. \nEndpoint: {Endpoint}. \nStatusCode: {StatusCode}. \nResponse summary:\n{ResponseSummary}",
                        endpointExportCandidates, (int)httpResponse.StatusCode, responseFormatted);
                }
                else
                {
                    //DeleteAll does not currently return deleted candidates
                    //deletedCandidates = await httpResponse.Content.ReadFromJsonAsync<List<Models.ExportCandidates.CandidateModel>?>();
                }
            }
            catch (HttpRequestException e)
            {
                string msg = ExceptionHelper.FormatEndpointException(e, endpointExportCandidates, httpClient);
                logger.LogError(msg);
                return false;
            }
            return true;
        }

        public static async Task<List<Models.ExportCandidates.CandidateModel>?> SaveLocalCandidatesToDatabase(ILogger logger, HttpClient httpClient, List<Models.ExportCandidates.CandidateModel>? candidates)
        {
            if (candidates == null || candidates.Count() == 0)
            {
                logger.LogWarning("No candidates to save locally.");
                return new List<Models.ExportCandidates.CandidateModel>();
            }

            string endpointExportCandidates = $"ExportCandidate/Many";
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            try
            {
                //logger.LogInformation($"---------------------------");
                //eSignUpSync.Helpers.JsonOutput.WriteExportCandidatesJson(logger, candidates);
                //logger.LogInformation($"---------------------------");

                //First remove existing candidates
                bool? candidatesDeleted = await DeleteLocalCandidatesFromDatabase(logger, httpClient);

                if (candidatesDeleted != true) {
                    logger.LogError("Error removing existing data.");

                    return new List<Models.ExportCandidates.CandidateModel>();
                }

                httpResponse = await httpClient.PostAsJsonAsync<List<Models.ExportCandidates.CandidateModel>?>(endpointExportCandidates, candidates);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();
                    string responseFormatted = ResponseFormatter.FormatResponseContent(responseContent, 10000);

                    logger.LogError("Error saving candidates to local database. \nEndpoint: {Endpoint}. \nStatusCode: {StatusCode}. \nResponse summary:\n{ResponseSummary}",
                        endpointExportCandidates, (int)httpResponse.StatusCode, responseFormatted);
                }
                else
                {
                    candidates = await httpResponse.Content.ReadFromJsonAsync<List<Models.ExportCandidates.CandidateModel>?>();
                }
            }
            catch (HttpRequestException e)
            {
                string msg = ExceptionHelper.FormatEndpointException(e, endpointExportCandidates, httpClient);
                logger.LogError(msg);
                return new List<Models.ExportCandidates.CandidateModel>();
            }

            return candidates ?? new();
        }

        public static async Task<List<Models.ExportCandidates.CandidateModel>?> SaveLocalCandidatesToFile(ILogger logger, HttpClient httpClient, List<Models.ExportCandidates.CandidateModel>? candidates)
        {
            if (candidates == null || candidates.Count() == 0)
            {
                logger.LogWarning("No candidates to save locally.");
                return new List<Models.ExportCandidates.CandidateModel>();
            }

            string endpointExportCandidates = $"ExportCandidate";
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            // Prepare json options for file output
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            // Ensure exports directory exists
            string importsDir = Path.Combine(Directory.GetCurrentDirectory(), "Imports");
            try
            {
                Directory.CreateDirectory(importsDir);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Failed to create imports directory '{importsDir}' - continuing without saving the file.");
            }

            // Save request payload to file for debugging/audit
            string requestFileName = Path.Combine(importsDir, $"ExportCandidates_Request_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
            try
            {
                string requestJson = JsonSerializer.Serialize(candidates, jsonOptions);
                await File.WriteAllTextAsync(requestFileName, requestJson);
                logger.LogInformation($"Saved import request payload to {requestFileName}");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Failed to write imports request payload to file '{requestFileName}'");
            }

            return candidates ?? new();
        }
    }
}
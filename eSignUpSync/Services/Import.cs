using eSignUpSync.Helpers;
using eSignUpSync.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
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

            CandidatesLocal = await SaveLocalCandidates(logger, httpClientLocalData, CandidatesESignUp);
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

        public static async Task<List<Models.ExportCandidates.CandidateModel>?> SaveLocalCandidates(ILogger logger, HttpClient httpClient, List<Models.ExportCandidates.CandidateModel>? candidates)
        {
            if (candidates == null || candidates.Count() == 0)
            {
                logger.LogWarning("No candidates to save locally.");
                return new List<Models.ExportCandidates.CandidateModel>();
            }

            string endpointExportCandidates = $"ExportCandidate";
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            try
            {
                logger.LogInformation($"---------------------------");
                eSignUpSync.Helpers.JsonOutput.WriteExportCandidatesJson(logger, candidates);
                logger.LogInformation($"---------------------------");

                httpResponse = await httpClient.PostAsJsonAsync<List<Models.ExportCandidates.CandidateModel>?>(endpointExportCandidates, candidates);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();
                    logger.LogError($"Error saving candidates to local database");
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
    }
}

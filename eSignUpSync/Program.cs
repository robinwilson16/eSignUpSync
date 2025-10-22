using eSignUpEBSAPI.Models;
using eSignUpSync.Data;
using eSignUpSync.Helpers;
using eSignUpSync.Models;
using eSignUpSync.Models.Candidates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Globalization;
using System.Net.Http;
using System.Reflection;

namespace eSignUpSync
{
    class Program
    {
        private static IConfiguration? _configuration { get; set; }
        private static ApplicationDbContext? _context;
        public static APIAccessToken? APIAccessToken { get; set; }

        public static List<CandidateModel>? CandidatesLocal { get; set; }
        public static List<CandidateModel>? CandidatesESignUp { get; set; }

        public static SettingsExportModel SettingsExport = new SettingsExportModel();
        public static SettingsESignUpModel SettingsESignUp = new SettingsESignUpModel();
        
        public static CultureInfo? CultureInfo { get; set; }

        static async Task<int> Main(string[] args)
        {
            //Add Logger
            using var loggerFactory = LoggingHelper.CreateLoggerFactory();
            ILogger logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("\neSignUp Sync");
            logger.LogInformation("=========================================\n");

            string? productVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            logger.LogInformation($"Version {productVersion}");
            logger.LogInformation($"Copyright Robin Wilson");

            string configFile = "appsettings.json";
            string? customConfigFile = null;
            if (args.Length >= 1)
            {
                customConfigFile = args[0];
            }

            if (!string.IsNullOrEmpty(customConfigFile))
            {
                configFile = customConfigFile;
            }

            Console.WriteLine($"\nUsing Config File {configFile}");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(configFile, optional: false);

            try
            {
                _configuration = builder.Build();
            }
            catch (Exception e)
            {
                logger.LogError($"Error: {e}");
                return 1;
            }

            logger.LogInformation($"\nSetting Locale To {_configuration["Locale"]}");

            //Set locale to ensure dates and currency are correct
            CultureInfo = new CultureInfo(_configuration["Locale"] ?? "en-GB");
            Thread.CurrentThread.CurrentCulture = CultureInfo;
            Thread.CurrentThread.CurrentUICulture = CultureInfo;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo;

            SettingsESignUp = _configuration.GetSection("eSignUp").Get<SettingsESignUpModel>() ?? new SettingsESignUpModel();
            SettingsExport = _configuration.GetSection("Export").Get<SettingsExportModel>() ?? new SettingsExportModel();

            if (string.IsNullOrEmpty(SettingsESignUp.Endpoint) || string.IsNullOrEmpty(SettingsExport.APIEndPoint))
            {
                logger.LogError("API Endpoint not configured. Please check the appsettings.json file.");
                return 1;
            }
            else
            {
                logger.LogInformation($"Using API Endpoint Located at {SettingsExport.APIEndPoint} for local data");
                logger.LogInformation($"Using API Endpoint Located at {SettingsESignUp.Endpoint} for eSignUp data");
            }

            //Set up HTTP Clients
            HttpClient httpClientLocalData = new HttpClient();
            httpClientLocalData.BaseAddress = new Uri(SettingsExport.APIEndPoint);
            HttpClient httpClientESignUp = new HttpClient();
            httpClientESignUp.BaseAddress = new Uri(SettingsESignUp.Endpoint);
            if (httpClientLocalData == null || httpClientESignUp == null)
            {
                logger.LogError("Error creating HTTP Client to connect to the APIs");
                return 1;
            }

            //Get Local Data
            CandidatesLocal = await Services.Export.GetLocalCandidates(logger, httpClientLocalData);
            logger.LogInformation($"\nLoaded Candidates from Local MIS System: {CandidatesLocal?.Count()}\n");
            if (CandidatesLocal == null || CandidatesLocal.Count() == 0)
            {
                logger.LogWarning("No candidates found in local MIS system. Exiting.");
                return 0;
            }

            //Get eSignUp API Access Token
            APIAccessToken = await Services.Export.GetESignUpAPIToken(logger, httpClientESignUp, SettingsESignUp);
            if (APIAccessToken == null || string.IsNullOrEmpty(APIAccessToken.Token))
            {
                logger.LogError("Failed to acquire eSignUp API Access Token. Exiting.");
                return 1;
            }
            else
            {
                logger.LogInformation($"Acquired API Access Token:\n{APIAccessToken.Token}\n");
            }

            //Set Authorization Header for eSignUp HTTP Client    
            httpClientESignUp.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", APIAccessToken.Token);

            //Get eSignUp Data
            CandidatesESignUp = await Services.Export.GetESignUpCandidates(logger, httpClientESignUp);
            logger.LogInformation($"\nLoaded Candidates from eSignUp: {CandidatesESignUp?.Count()}\n");
            if (CandidatesESignUp == null || CandidatesESignUp.Count() == 0)
            {
                logger.LogWarning("No candidates found in eSignUp");
                //return 0;
            }
            
            //Send Local Candidates to eSignUp
            int? totalCandidates = CandidatesLocal?.Count() ?? 0;
            int? recordsSent = await Services.Export.SendLocalCandidates(logger, httpClientESignUp, CandidatesLocal);

            logger.LogInformation($"\nTotal Candidates Sent to eSignUp: {recordsSent}\n");

            if (recordsSent == null || recordsSent < totalCandidates)
            {
                logger.LogWarning("Some candidates may not have been sent successfully. Please check the logs for details.");
                return 1;
            }

            return 0;
        }
    }
}
using eSignUpEBSAPI.Models;
using eSignUpSync.Data;
using eSignUpSync.Helpers;
using eSignUpSync.Models;
using eSignUpSync.Models.Candidates;
using eSignUpSync.Services;
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

            //Get eSignUp API Access Token
            APIAccessToken = await Services.Shared.GetESignUpAPIToken(logger, httpClientESignUp, SettingsESignUp);
            if (APIAccessToken == null || string.IsNullOrEmpty(APIAccessToken.Token))
            {
                logger.LogError("Failed to acquire eSignUp API Access Token. Exiting.");
                return 1;
            }
            else
            {
                logger.LogInformation($"Acquired API Access Token:\n{APIAccessToken.Token}\n");
            }

            bool? isError = false;

            //Perform Import to import eSignUp data to local eSignUp Database
            int? importResult = await Services.Import.DoImport(logger, httpClientLocalData, httpClientESignUp, APIAccessToken);
            if (importResult == null || importResult > 0)
            {
                logger.LogError("Import Failed");
                isError = true;
            }
            else
            {
                logger.LogInformation($"DoImport Process Finished.");
            }

            //Perform Export to export local data to eSignUp
            int? exportResult = await Export.DoExport(logger, httpClientLocalData, httpClientESignUp, APIAccessToken);
            if (exportResult == null || exportResult > 0)
            {
                logger.LogError("Export Failed");
                isError = true;
            }
            else
            {
                logger.LogInformation($"DoExport Process Finished.");
            }
            
            if (isError == true)
            {
                logger.LogError("eSignUp Sync Finished with Errors.");
                return 1;
            }
            else
            {
                logger.LogInformation($"eSignUp Sync Process Finished.");
            }
            
            return 0;
        }
    }
}
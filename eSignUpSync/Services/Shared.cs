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
    public class Shared
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
    }
}

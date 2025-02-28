using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace EmailReaderBackend.Services
{
    public class EmailService
    {
        private readonly string _clientId = "clientId";
        private readonly string _tenantId = "consumers";
        private readonly string[] _scopes = { "https://graph.microsoft.com/Mail.Read", "https://graph.microsoft.com/User.Read" };
        private readonly string _cacheFileName = "msal_cache.dat";
        private IPublicClientApplication _app;
        private GraphServiceClient _graphClient;

        public EmailService()
        {
            _app = PublicClientApplicationBuilder
                .Create(_clientId)
                .WithTenantId(_tenantId)
                .WithRedirectUri("http://localhost:3000/callback")
                .Build();

            var storageProperties = new StorageCreationPropertiesBuilder(
                _cacheFileName, 
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
                .WithMacKeyChain("MSALCache", "MSALCache")
                .Build();

            var cacheHelper = MsalCacheHelper.CreateAsync(storageProperties).Result;
            cacheHelper.RegisterCache(_app.UserTokenCache);
        }

        public async Task<string> AuthenticateAsync()
        {
            var accounts = await _app.GetAccountsAsync();
            var account = accounts.FirstOrDefault();

            try
            {
                var authResult = await _app.AcquireTokenSilent(_scopes, account)
                                           .ExecuteAsync();
                return authResult.AccessToken;
            }
            catch (MsalUiRequiredException)
            {
                var authResult = await _app.AcquireTokenInteractive(_scopes)
                                           .WithAccount(account)
                                           .ExecuteAsync();
                return authResult.AccessToken;
            }
        }

        // Méthode pour afficher la barre de progression
        private void ShowProgress(int current, int total)
        {
            int progressBarWidth = 50; 
            double percentage = (double)current / total;
            int progressBlocks = (int)(percentage * progressBarWidth);
            string progress = new string('#', progressBlocks) + new string('-', progressBarWidth - progressBlocks);

            Console.Write($"\r[{progress}] {current}/{total} emails récupérés...");
        }
        public async Task<string> GetEmailsAsync()
        {
            string token = await AuthenticateAsync();
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _graphClient = new GraphServiceClient(httpClient);

            List<object> allEmails = new List<object>();
            int currentEmailCount = 0;

            try
            {
                // Récupérer la première page des emails
                var page = await _graphClient.Me.Messages
                    .GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Select = new[] { "subject", "receivedDateTime", "from" };
                        requestConfig.QueryParameters.Orderby = new[] { "receivedDateTime DESC" }; // Trier par date
                    });

                // Tant qu'il y a des pages suivantes
                while (page?.Value != null)
                {
                    allEmails.AddRange(page.Value.Select(email => new
                    {
                        From = email.From?.EmailAddress?.Name,
                        Subject = email.Subject,
                        Received = email.ReceivedDateTime?.ToString("yyyy-MM-dd HH:mm:ss")
                    }));

                    currentEmailCount += page.Value.Count;

                    // Afficher la progression
                    ShowProgress(currentEmailCount, currentEmailCount + page.OdataNextLink?.Length ?? 0);

                    // Récupérer la page suivante si elle existe
                    page = page.OdataNextLink != null ? await _graphClient.Me.Messages
                        .WithUrl(page.OdataNextLink)
                        .GetAsync() : null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nErreur lors de la récupération des emails: {ex.Message}");
            }

            Console.WriteLine();

            // Convertir la liste des emails en JSON
            string jsonResult = JsonConvert.SerializeObject(allEmails, Formatting.Indented);
            return jsonResult;
        }

    }
}
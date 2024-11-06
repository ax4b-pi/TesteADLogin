using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using TesteADLogin.Model;

namespace TesteADLogin.Services
{
    public class AzureAdAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IPublicClientApplication _app;

        public AzureAdAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _app = PublicClientApplicationBuilder
                .Create(_configuration["AzureAd:ClientId"])
                .WithAuthority(AzureCloudInstance.AzurePublic, _configuration["AzureAd:TenantId"])
                .Build();
        }

        public async Task<UserInfo> AuthenticateUserAsync(string username, string password)
        {
            try
            {
                var scopes = new[] { "User.Read" };
                var securePassword = new SecureString();
                foreach (char c in password) securePassword.AppendChar(c);

                var result = await _app.AcquireTokenByUsernamePassword(scopes, username, securePassword)
                    .ExecuteAsync();

                // Here you can make additional Graph API calls to get more user info
                var userInfo = new UserInfo
                {
                    Id = result.Account.HomeAccountId.ObjectId,
                    DisplayName = result.Account.Username,
                    Email = result.Account.Username
                };

                return userInfo;
            }
            catch (MsalException)
            {
                return null;
            }
        }
    }
}

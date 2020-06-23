using Common;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TeamServer.Models;

namespace TeamServer.Tests
{
    class TestClient
    {
        
        public static HttpClient HttpClient { get; set; }

        public TestClient()
        {
            var testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient = testServer.CreateClient();
            Program.StartTeamServer("a");
        }

        internal static async Task<ClientAuthenticationResult> ClientLogin(string nick, string pass)
        {
            var authRequest = new ClientAuthenticationRequest { Nick = nick, Password = pass };
            var response = await HttpClient.PostAsync("api/Client", Serialisation.SerialiseData(authRequest));
            var result = Serialisation.DeserialiseData<ClientAuthenticationResult>(response.Content.ReadAsStringAsync());

            if (result.Result == ClientAuthenticationResult.AuthResult.LoginSuccess)
            {
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
            }

            return result; 
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TeamServer.Models;
using Xunit;

namespace TeamServer.Tests.ClientTests
{
    public class Client
    {
        public Client()
        {
            new TestClient();
        }

        [Fact]
        public async void LoginSuccess()
        {
            var result = await TestClient.ClientLogin(TeamServer.Helpers.GeneratePseudoRandomString(6), "a");

            Assert.Equal(ClientAuthenticationResult.AuthResult.LoginSuccess, result.Result);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async void BadPassword()
        {
            var result = await TestClient.ClientLogin(TeamServer.Helpers.GeneratePseudoRandomString(6), "a");

            Assert.Equal(ClientAuthenticationResult.AuthResult.BadPassword, result.Result);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async void NickInUse()
        {
            var user = TeamServer.Helpers.GeneratePseudoRandomString(6);

            await TestClient.ClientLogin(user, "a");
            var result = await TestClient.ClientLogin(user, "a");

            Assert.Equal(ClientAuthenticationResult.AuthResult.NickInUse, result.Result);
            Assert.NotNull(result.Token);
        }

        [Theory]
        [InlineData("plaintext", "")]
        [InlineData("", "a")]
        [InlineData("", "b")]
        public async void InvalidRequest(string nick, string pass)
        {
            var result = await TestClient.ClientLogin(nick, pass);

            Assert.Equal(ClientAuthenticationResult.AuthResult.InvalidRequest, result.Result);
            Assert.NotNull(result.Token);
        }
    }
}

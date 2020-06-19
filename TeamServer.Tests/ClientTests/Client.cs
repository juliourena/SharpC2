using System;
using System.Collections.Generic;
using System.Text;
using TeamServer.Models;
using Xunit;

namespace TeamServer.Tests.ClientTests
{
    public class Client
    {
        [Fact]
        public void SuccessfullClientLogin()
        {
            var request = new ClientAuthenticationRequest { Nick = "plaintext", Password = "a" };
            var results = Controllers.ClientController.ClientLogin(request);

            Assert.Equal(ClientAuthenticationResult.AuthResult.LoginSuccess, results.Result);
            Assert.NotNull(results.Token);
        }

        [Fact]
        public void BadPasswordClientLogin()
        {
            var request = new ClientAuthenticationRequest { Nick = "plaintext", Password = "23" };
            var results = Controllers.ClientController.ClientLogin(request);

            Assert.Equal(ClientAuthenticationResult.AuthResult.BadPassword, results.Result);
            Assert.Null(results.Token);
        }

        [Fact]
        public void NickInUse()
        {
            var request = new ClientAuthenticationRequest { Nick = "plaintext", Password = "a" };
            Controllers.ClientController.ClientLogin(request);
            var results = Controllers.ClientController.ClientLogin(request);
            
            Assert.Equal(ClientAuthenticationResult.AuthResult.NickInUse, results.Result);
            Assert.Null(results.Token);
        }

        [Fact]
        public void InvalidRequest()
        {
            var request = new ClientAuthenticationRequest { Nick = "plaintext", Password = "" };
            var results = Controllers.ClientController.ClientLogin(request);
            Controllers.ClientController.ClientLogin(request);

            Assert.Equal(ClientAuthenticationResult.AuthResult.InvalidRequest, results.Result);
            Assert.Null(results.Token);
        }


    }
}

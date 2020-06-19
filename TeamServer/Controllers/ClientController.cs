using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Models;

namespace TeamServer.Controllers
{
    public class ClientController
    {
        private static List<string> _connectedClients { get; set; } = new List<string>();

        public static ClientAuthenticationResult ClientLogin(ClientAuthenticationRequest request)
        {
            var result = new ClientAuthenticationResult();

            if (string.IsNullOrEmpty(request.Nick) || string.IsNullOrEmpty(request.Password))
            {
                result.Result = ClientAuthenticationResult.AuthResult.InvalidRequest;
            }
            else if (!AuthenticationController.ValidatePassword(request.Password))
            {
                result.Result = ClientAuthenticationResult.AuthResult.BadPassword;
            }
            else if (_connectedClients.Contains(request.Nick, StringComparer.OrdinalIgnoreCase))
            {
                result.Result = ClientAuthenticationResult.AuthResult.NickInUse;
            }
            else
            {
                result.Result = ClientAuthenticationResult.AuthResult.LoginSuccess;
                result.Token = AuthenticationController.GenerateAuthenticationToken(request.Nick);

                AddNewClient(request.Nick);
            }

            return result;
        }

        public static List<string> GetConnectedClient()
        {
            return _connectedClients;
        }

        private static void AddNewClient(string nick)
        {
            _connectedClients.Add(nick);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Models
{
    public class ClientAuthenticationResult
    {
        public AuthResult Result { get; set; }

        public string Token { get; set; }

        public enum AuthResult
        {
            LoginSuccess,
            BadPassword,
            NickInUse,
            InvalidRequest
        }
    }
}

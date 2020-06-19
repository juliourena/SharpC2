using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Models
{
    public class ClientAuthenticationRequest
    {
        public string Nick { get; set; }
        public string Password { get; set; }
    }
}

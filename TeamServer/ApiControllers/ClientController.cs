using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamServer.Controllers;
using TeamServer.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamServer.ApiControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        // GET: api/<ClientController>
        [HttpGet]
        public List<string> Get()
        {
            return Controllers.ClientController.GetConnectedClient();
        }

        [AllowAnonymous]
        // POST api/<ClientController>
        [HttpPost]
        public ClientAuthenticationResult ClientLogin([FromBody] ClientAuthenticationRequest request)
        {
            return Controllers.ClientController.ClientLogin(request);
        }
    }
}

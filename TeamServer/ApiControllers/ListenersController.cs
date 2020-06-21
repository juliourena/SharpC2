using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamServer.Listeners;
using TeamServer.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TeamServer.ApiControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ListenersController : ControllerBase
    {
        // GET: api/<ListenersController>
        [HttpGet]
        public IEnumerable<ListenerBase> GetListeners()
        {
            return Program.ServerController.ListenerController.GetListeners();
        }

        [HttpPost("http")]
        public IEnumerable<ListenerBase> NewHttpListener([FromBody]NewHttpListenerRequest request)
        {
            return Program.ServerController.ListenerController.StartHttpListener(request);
        }

        //[HttpPost("http")]
        //public void NewHttpListener([FromBody] NewHttpListenerRequest request)
        //{
        //    Program.ServerController.ListenerController.StartHttpListener(request);
        //}

        [HttpPost("tcp")]
        public void NewTcpListener([FromBody]NewTcpListenerRequest request)
        {
            Program.ServerController.ListenerController.StartTcpListener(request);
        }

        // DELETE api/<ListenersController>/5
        [HttpDelete("{id}")]
        public void StopListener(string id)
        {
            Program.ServerController.ListenerController.StopListener(id);
        }
    }
}

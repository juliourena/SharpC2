using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamServer.Models;

namespace TeamServer.ApiControllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PayloadController : ControllerBase
    {
        [HttpPost]
        public PayloadResponse GeneratePayload([FromBody]PayloadRequest request)
        {
            return Controllers.PayloadControllerBase.GenerateAgentPayload(request);
        }
    }
}

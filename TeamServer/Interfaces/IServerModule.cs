using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Controllers;
using TeamServer.Models.ModuleModels;

namespace TeamServer.Interfaces
{
    public interface IServerModule
    {
        void Init(ServerController server, AgentController agent);
        ServerModule GetModuleInfo();
    }
}

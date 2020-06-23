using AgentCore.Models;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Controllers;
using TeamServer.Interfaces;
using TeamServer.Models.ModuleModels;

namespace TeamServer.Modules
{
    public class CoreServerModule : IServerModule
    {
        private ServerController Server { get; set; }
        private AgentController Agent { get; set; }
        public void Init(ServerController server, AgentController agent)
        {
            Server = server;
            Agent = agent;
        }

        public ServerModule GetModuleInfo()
        {
            return new ServerModule
            {
                Name = "Core",
                Description = "Handles the minimum core server functionality.",
                Developers = new List<Developer>
                {
                    new Developer
                    {
                        Name = "Julio Urena",
                        Handle = "plaintext"
                    }
                },
                ServerCommands = new List<ServerCommand>
                {
                    new ServerCommand
                    {
                        Name = "AgentCheckIn",
                        Description = "Handles Agent Checkin",
                        CallBack = HandleAgentCheckIn
                    }
                }
            };
        }

        private void HandleAgentCheckIn(Metadata metadata, C2Data c2Data)
        {
            Agent.UpdateSession(metadata);
        }
    }
}

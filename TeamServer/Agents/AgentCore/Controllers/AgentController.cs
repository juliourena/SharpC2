using AgentCore.Interfaces;
using AgentCore.Models;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;

namespace AgentCore.Controllers
{
    public class AgentController
    {
        private ConfigController Config { get; set; }
        private ICommModule CommModule { get; set; }
        public AgentStatus AgentStatus { get; private set; }

        public AgentController(ConfigController config, ICommModule commModule)
        {
            AgentStatus = AgentStatus.Starting;
            Config = config;
            CommModule = commModule;
        }

        public void Start()
        {
            AgentStatus = AgentStatus.Running;

            CommModule.Start();

            while (AgentStatus == AgentStatus.Running)
            {
                if (CommModule.RecvData() == true)
                {
                    
                }
            }
        }

        public void Stop()
        {
            AgentStatus = AgentStatus.Stopeed;
        }
    }
}

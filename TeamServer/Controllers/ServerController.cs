using AgentCore.Models;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Interfaces;
using TeamServer.Models.ModuleModels;

namespace TeamServer.Controllers
{
    public class ServerController
    {
        public ServerStatus ServerStatus { get; private set; }
        public ListenerControllerBase ListenerController { get; private set; }
        public AgentController AgentController { get; set; }
        private List<ServerModule> ServerModules { get; set; }

        public delegate void OnServerCommand(Metadata metadata, C2Data c2Data);
        
        public ServerController()
        {
            ServerStatus = ServerStatus.Starting;
            ListenerController = new ListenerControllerBase();
            AgentController = new AgentController();
            ServerModules = new List<ServerModule>();
        }

        public void RegisterServerModule(IServerModule module)
        {
            module.Init(this, AgentController);
            ServerModules.Add(module.GetModuleInfo());
        }

        public void Start()
        {
            ServerStatus = ServerStatus.Running;

            Task.Factory.StartNew(delegate ()
            {
                while (ServerStatus == ServerStatus.Running)
                {
                    var commModules = ListenerController.HTTPListeners.ToList();
                    foreach (var commModule in commModules)
                    {
                        if (commModule.RecvData(out Metadata metadata, out C2Data c2Data))
                        {
                            HandleC2Data(metadata, c2Data);
                        }
                    }
                }
            });
        }

        private void HandleC2Data(Metadata metadata, C2Data c2Data)
        {
            var callback = ServerModules
                .Where(m => m.Name.Equals(c2Data.Module, StringComparison.OrdinalIgnoreCase))
                .Select(m => m.ServerCommands).FirstOrDefault()
                .Where(c => c.Name.Equals(c2Data.Command, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.CallBack).FirstOrDefault();

            callback?.Invoke(metadata, c2Data);
        }

        public void Stop()
        {
            ServerStatus = ServerStatus.Stopped;
        }

    }

    public enum ServerStatus
    {
        Starting,
        Running,
        Stopped
    }
}

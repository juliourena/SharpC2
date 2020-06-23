using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Controllers;

namespace TeamServer.Models.ModuleModels
{
    public class ServerCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ServerController.OnServerCommand CallBack { get; set; }
    }
}

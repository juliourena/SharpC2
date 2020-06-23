using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Models.ModuleModels
{
    public class ServerModule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Developer> Developers { get; set; }
        public List<ServerCommand> ServerCommands { get; set; }
    }
}

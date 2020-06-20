using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Listeners
{
    public class ListenerBase
    {
        public string ListenerId { get; set; } = Helpers.GeneratePseudoRandomString(8);
        public int BindPort { get; set; }
    }
}

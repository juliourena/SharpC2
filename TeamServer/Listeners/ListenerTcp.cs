using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Listeners
{
    public class ListenerTcp : ListenerBase
    {
        public string BindAddress { get; set; }
    }
}

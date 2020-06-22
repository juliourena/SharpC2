using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamServer.Models;

namespace TeamServer.Listeners
{
    public class ListenerHttp : ListenerBase
    {        
        public string ConnectAddress { get; set; }
        public int ConnectPort { get; set; }
        public HttpTrafficProfile TrafficProfile{ get; set; }
    }
}

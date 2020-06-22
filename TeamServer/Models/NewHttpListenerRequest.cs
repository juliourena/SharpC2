using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Models
{
    public class NewHttpListenerRequest
    {
        public int BindPort { get; set; }

        public string ConnectAddress { get; set; }

        public int ConnectPort { get; set; }
        public HttpTrafficProfile TrafficProfile { get; set; }
    }
}

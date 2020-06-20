using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Models
{
    public class NewTcpListenerRequest
    {
        public string BindAddress { get; set; }
        public int BindPort { get; set; }
    }
}

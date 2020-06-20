using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TeamServer.Interfaces;
using TeamServer.Listeners;

namespace TeamServer.Modules
{
    public class HTTPCommModule : ICommModule
    {
        public ListenerHttp Listener { get; set; }
        private Socket Socket { get; set; }

        public void Init()
        {
            Socket = new Socket(SocketType.Stream, ProtocolType.IP);
        }

        public void Start()
        {
            Socket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), Listener.BindPort));
            Socket.Listen(20);
        }

        public void Stop()
        {
            Socket.Close();
            Socket.Dispose();
        }
    }
}

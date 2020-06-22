using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TeamServer.Listeners;
using TeamServer.Models;
using TeamServer.Modules;

namespace TeamServer.Controllers
{
    public class ListenerControllerBase
    {
        public List<HTTPCommModule> HTTPListeners { get; set; } = new List<HTTPCommModule>();
        public List<ListenerTcp> TCPListeners { get; set; } = new List<ListenerTcp>();
        public IEnumerable<ListenerBase> StartHttpListener(NewHttpListenerRequest request)
        {
            if (!IPAddress.TryParse(request.ConnectAddress, out _))
            {
                throw new ArgumentException("Invalid IP Address");
            }
            var listener = new ListenerHttp
            {
                Type = ListenerType.HTTP,
                BindPort = request.BindPort,
                ConnectAddress = request.ConnectAddress,
                ConnectPort = request.ConnectPort,
                TrafficProfile = request.TrafficProfile ?? new HttpTrafficProfile() // if trafficprofile is null them, create a new instance and asign it. 
            };

            var module = new HTTPCommModule
            {
                Listener = listener
            };

            HTTPListeners.Add(module);
            
            module.Init();
            module.Start();

            var result = new List<ListenerBase>();
            foreach (var value in HTTPListeners)
            {
                result.Add(value.Listener);
            }

            return result;
        }

        //public void StartHttpListener(NewHttpListenerRequest request)
        //{
        //    var listener = new ListenerHttp
        //    {
        //        BindPort = request.BindPort,
        //        ConnectAddress = request.ConnectAddress,
        //        ConnectPort = request.ConnectPort
        //    };

        //    var module = new HTTPCommModule
        //    {
        //        Listener = listener
        //    };

        //    HTTPListeners.Add(module);

        //    module.Init();
        //    module.Start();
        //}

        public void StartTcpListener(NewTcpListenerRequest request)
        {
            var listener = new ListenerTcp
            {
                BindAddress = request.BindAddress,
                BindPort = request.BindPort
            };

            TCPListeners.Add(listener);

            
        }

        public IEnumerable<ListenerBase> GetListeners()
        {
            var result = new List<ListenerBase>();

            foreach (var listener in TCPListeners)
            {
                result.Add(listener);
            }

            foreach (var module in HTTPListeners)
            {
                result.Add(module.Listener);
            }

            return result;
        }

        public void StopListener(string listenerId)
        {
            var tcplisteners = TCPListeners.Where(l => l.ListenerId.Equals(listenerId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (tcplisteners != null)
            {
                TCPListeners.Remove(tcplisteners);
            }
            else
            {
                var httpModule = HTTPListeners.Where(l => l.Listener.ListenerId.Equals(listenerId, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (httpModule != null)
                {
                    HTTPListeners.Remove(httpModule);
                    httpModule.Stop(); // PlanText - added to Stop the listener. 
                }
            }

        }
    }
}

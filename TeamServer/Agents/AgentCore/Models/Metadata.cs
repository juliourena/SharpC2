using System;
using System.Collections.Generic;
using System.Text;

namespace AgentCore.Models
{
    public class Metadata
    {
        //public Metadata()
        //{
        //    AgentId = "asfsadf";
        //    Hostname = Helpers.GetHostname;
        //    IPAddress = Helpers.GetIpAddress;
        //    Identity = Helpers.GetIdentity;
        //    ProcessName = Helpers.GetProcessName;
        //    ProcessId = Helpers.GetProcessId;
        //    Arch = Helpers.GetArch;
        //    Integrity = Helpers.GetIntegrity;
        //}

        public string AgentId { get; set; }
        public string Hostname { get; set; }
        public string IPAddress { get; set; }
        public string Identity { get; set; }
        public string ProcessName { get; set; }
        public int ProcessId { get; set; }
        public Arch Arch { get; set; }
        public Integrity Integrity { get; set; }
    }

    public enum Arch
    {
        x64,
        x86
    }

    public enum Integrity
    {
        Medium,
        High,
        SYSTEM
    }
}

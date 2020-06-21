using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace TeamServer.Models
{
    public class PayloadRequest
    {
        public string  ListenerId { get; set; }
        public string AssemblyName { get; set; }

        public TargetFramework TargetFramework { get; set; }

        public OutputType OutputType { get; set; }
    }
    public enum TargetFramework
    {
        Net35,
        Net40
    }

    public enum OutputType
    {
        Exe = 0,
        Dll = 2
    }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Models
{
    public class PayloadResponse
    {
        public CompilerStatus CompilerStatus { get; set; }
        public string ErrorMessage { get; set; }
        public string EncodedAssembly { get; set; }
    }

    public enum CompilerStatus
    {
        Success,
        Fail
    }
}

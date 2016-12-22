using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Components
{
    public class AppSettings
    {
        const string KEY_LOG = "log";
        /// <summary>
        /// 是否记录日志
        /// </summary>
        public static bool Log
        {
            get
            {
                var val = Get(KEY_LOG);
                bool result = false;
                Boolean.TryParse(val, out result);
                return result;
            }
        }

        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key) || !ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return string.Empty;
            }
            return ConfigurationManager.AppSettings[key].ToString();
        }
    }
}

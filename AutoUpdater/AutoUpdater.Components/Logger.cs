using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace AutoUpdater.Components
{
    public class Logger<T>
    {
        ILog _logger = null;
        public Logger()
        {
            _logger = LogManager.GetLogger(typeof(T));
        }

        public void LogError(string msg, Exception ex = null)
        {
            if (_logger == null)
            {
                return;
            }
            _logger.Error(msg, ex);
        }

        public void LogError(Exception ex)
        {
            if (_logger == null)
            {
                return;
            }
            _logger.Error("Error:", ex);
        }

        public void LogInfo(string msg, Exception ex = null)
        {
            if (_logger == null || !AppSettings.Log)
            {
                return;
            }
            _logger.Info(msg, ex);
        }

        public void LogInfo(Exception ex)
        {
            if (_logger == null || !AppSettings.Log)
            {
                return;
            }
            _logger.Info("Info:", ex);
        }
    }
}

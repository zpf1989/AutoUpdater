using AutoUpdater.Components;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Host
{
    public partial class HostService : ServiceBase
    {
        private static Logger<HostService> _logger = new Logger<HostService>();
        private static List<IDisposable> _instances = new List<IDisposable>();
        public HostService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var app = WebApp.Start<Startup>(url: HostContext.HostUrl);
                _instances.Add(app);
                _logger.LogInfo(string.Format("{0}已启动，host：{1}", HostContext.SvrName, HostContext.HostUrl));
            }
            catch (Exception ex)
            {
                _logger.LogInfo(string.Format("{0}启动失败，host：{1}", HostContext.SvrName, HostContext.HostUrl), ex);
                Console.WriteLine(string.Format("{0}启动失败，请查看日志", HostContext.SvrName));
                base.Stop();
            }
        }

        protected override void OnStop()
        {
            if (_instances != null && _instances.Count > 0)
            {
                _logger.LogInfo("将要释放所有web应用...");
                _instances.ForEach(i => i.Dispose());
                _logger.LogInfo("所有web应用释放完毕");
            }
            _logger.LogInfo(HostContext.SvrName + "已停止");
        }
    }
}

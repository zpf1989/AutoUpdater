using AutoUpdater.Components;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AutoUpdater.Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        Logger<App> _logger = new Logger<App>();
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //检查更新
            CheckUpdate();
        }

        private void CheckUpdate()
        {
            var rst = HttpUtils.GetResult(string.Format("{0}/api/client/check?appid={1}&v2={2}",
                ClientContext.SvrUrl.TrimEnd('/', '\\'),
                ClientContext.AppId,
                ClientContext.Version));
            if (rst.code == ResultCode.NewVersion)
            {
                MessageBoxResult msgRst = MessageBox.Show(string.Format("发现新版本[{0}]，当前版本[{1}]，建议升级到最新版本。\r\n是否升级？",
                    rst.data.version, ClientContext.Version),
                    "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgRst != MessageBoxResult.Yes)
                {
                    this.Shutdown(-1);
                    return;
                }
                //关闭目标程序
                CloseTargetApp();
                //升级
                var winRst = new MainWindow().ShowDialog();
                if (winRst == true)
                {
                    //升级成功，启动目标程序
                    try
                    {
                        Process.Start(ClientContext.TargetAppFullName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("启动主程序失败，主程序目录：" + ClientContext.TargetAppFullName, ex);
                        MessageBox.Show("启动主程序失败，请查看日志！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("升级失败！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            this.Shutdown(-1);
        }

        private void CloseTargetApp()
        {
            Process[] processes = Process.GetProcessesByName(ClientContext.Target.Replace(".exe", ""));
            if (processes == null || processes.Length < 1)
            {
                return;
            }
            var targetP = processes[0];
            if (targetP != null)
            {
                if (!targetP.CloseMainWindow())
                {
                    targetP.Kill();
                }
            }
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var rst = MessageBox.Show(e.Exception.Message + Environment.NewLine + "是否退出？", "程序错误", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rst == MessageBoxResult.Yes)
            {
                this.Shutdown(-1);
            }
        }
    }
}

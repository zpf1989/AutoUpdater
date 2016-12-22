using AutoUpdater.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AutoUpdater.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Logger<MainWindow> _logger = new Logger<MainWindow>();
        private static string upgradePath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('/', '\\') + "/Upgrade";
        private static string zipFile = upgradePath + "/upgrade.zip";
        private static string tempPath = upgradePath + "/temp";
        private DispatcherTimer _cntTimer = new DispatcherTimer();
        int timeCnt = 5;
        public MainWindow()
        {
            InitializeComponent();

            _cntTimer.Interval = new TimeSpan(0, 0, 1);
            _cntTimer.Tick += (o, e) =>
            {
                if (!_cntTimer.IsEnabled)
                {
                    return;
                }
                txtTimeCnt.Text = (--timeCnt).ToString() + "s";
            };
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //启用鼠标拖拽
            e.Handled = true;
            this.DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //屏蔽Alt+F4
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Upgrade();
        }

        private void Upgrade()
        {
            linkRetry.IsEnabled = false;
            Notify("正在下载升级包，请稍后...");
            HttpUtils.Download(string.Format("{0}/api/client/upgrade", ClientContext.SvrUrl.TrimEnd('/', '\\')), zipFile,
                    downloadProgressChangeCallback: o =>
                    {
                        string msg = string.Format("下载进度：{0}\t{1}/{2}", o.ProgressPercentage, o.BytesReceived, o.TotalBytesToReceive);
                        Notify(msg, false);
                        progressBar.Value = o.ProgressPercentage;
                    },
                    downloadCompleteCallback: async o =>
                     {
                         if (o.Error != null)
                         {
                             Notify("升级包下载失败！");
                             return;
                         }
                         //解压文件并应用更新
                         await UnZipAndProcess();
                     });
        }

        private async Task UnZipAndProcess()
        {
            Notify("开始解压升级包upgrade.zip...");
            ZipCompresser.Decompress(zipFile, tempPath);
            Notify("升级包upgrade.zip解压完毕");
            //1、获取更新配置文件
            var upgradeConf = tempPath + "/files.json";
            if (!File.Exists(upgradeConf))
            {
                Notify("未找到升级配置文件，升级包不完整！");
                return;
            }

            var confStr = File.ReadAllText(upgradeConf);
            var confs = JsonConvert.DeserializeObject<IList<UpgradeConf>>(confStr);
            var progress = 0;
            for (var idx = 0; idx < confs.Count; idx++)
            {
                var conf = confs[idx];
                var from = conf.path.Replace("~", tempPath);
                var to = conf.path.Replace("~", ClientContext.TargetAppPath);
                _logger.LogInfo(string.Format("开始\t——\t{0}", conf.ToString()));
                if (conf.type == ItemType.file)
                {
                    HandleFile(from, to, conf);
                }
                else
                {
                    HandleFolder(to, conf);
                }
                progress = Convert.ToInt32(((idx + 1) * 1.0 / confs.Count * 100));
                progressBar.Value = progress;
                Notify(string.Format("正在应用更新：{0},{1}", (progress + "/%"), conf.path), false);
                _logger.LogInfo(string.Format("结束\t——\t{0}", conf.ToString()));
            }
            Notify("更新完毕！将在5s后重新启动主程序。");
            linkRetry.IsEnabled = true;
            _cntTimer.Start();
            await Task.Delay(5000);
            this.DialogResult = true;
            this.Close();
        }

        private void HandleFile(string from, string to, UpgradeConf conf)
        {
            //文件
            if (!File.Exists(from) && conf.opt != OptType.delete)
            {
                _logger.LogInfo(string.Format("未找到升级文件from：{0}", from));
                return;
            }

            if (File.Exists(to))
            {
                _logger.LogInfo(string.Format("删除文件：{0}", to));
                try
                {
                    File.Delete(to);
                }
                catch (Exception ex)
                {
                    _logger.LogInfo(string.Format("删除文件：{0}失败", to), ex);
                }
            }
            if (conf.opt != OptType.delete)
            {
                var dir = new FileInfo(to).Directory;
                if (!dir.Exists)
                {
                    _logger.LogInfo(string.Format("创建目录：{0}", dir.FullName));
                    dir.Create();
                }
                _logger.LogInfo(string.Format("复制文件：from\t{0}\tto\t{1}", from, to));
                try
                {
                    File.Copy(from, to);
                }
                catch (Exception ex)
                {
                    _logger.LogInfo(string.Format("复制文件：{0}失败", to), ex);
                }
            }
        }

        private void HandleFolder(string to, UpgradeConf conf)
        {
            if (conf.opt == OptType.delete && Directory.Exists(to))
            {
                _logger.LogInfo(string.Format("删除目录：{0}", to));
                try
                {
                    Directory.Delete(to, true);
                }
                catch (Exception ex)
                {
                    _logger.LogInfo(string.Format("删除目录：{0}失败", to), ex);
                }
            }
            else if (conf.opt == OptType.add && !Directory.Exists(to))
            {
                _logger.LogInfo(string.Format("添加目录：{0}", to));
                Directory.CreateDirectory(to);
            }
        }

        private void Notify(string msg, bool log = true)
        {
            txtInfo.Text = msg;
            if (log)
            {
                _logger.LogInfo(msg);
            }
        }

        private void linkRetry_Click(object sender, RoutedEventArgs e)
        {
            Upgrade();
        }
    }
}

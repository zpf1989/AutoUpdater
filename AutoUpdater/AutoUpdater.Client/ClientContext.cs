using AutoUpdater.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Client
{
    public class ClientContext
    {
        static ClientContext()
        {
            NewestVersion = Version;
        }

        public static string SvrUrl
        {
            get
            {
                return AppSettings.Get("svrurl");
            }
        }

        public static string AppId
        {
            get
            {
                return AppSettings.Get("appid");
            }
        }

        public static string Version
        {
            get
            {
                return AppSettings.Get("version");
            }
        }
        public static string Target
        {
            get
            {
                return AppSettings.Get("target");
            }
        }

        public static string TargetAppPath
        {
            get
            {
                return new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName.TrimEnd('/', '\\');
            }
        }

        public static string TargetAppFullName
        {
            get
            {
                return TargetAppPath + "/" + Target;
            }
        }

        public static string NewestVersion { get; set; }
    }
}

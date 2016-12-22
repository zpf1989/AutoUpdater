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
        public static string SvrUrl
        {
            get
            {
                return AppSettings.Get("svrurl");
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
    }
}

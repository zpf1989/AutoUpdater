
using MyNet.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Host
{
    public static class HostContext
    {
        static HostContext()
        {

        }

        static string _defaultSvrName = "AutoUpdaterHost";
        static string _svrName = "";
        public static string SvrName
        {
            get
            {
                if (string.IsNullOrEmpty(_svrName))
                {
                    var val = AppSettingUtils.Get("svrname");
                    _svrName = string.IsNullOrEmpty(val) ? _defaultSvrName : val;
                }

                return _svrName;
            }
        }

        public static string HostUrl
        {
            get
            {
                var url = AppSettingUtils.Get("host");
                return url;
            }
        }

        public static string RootPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static MediaTypeFormatter CurrentMediaTypeFormatter
        {
            get; set;
        }

        /// <summary>
        /// 是否调试
        /// </summary>
        public static bool Debug
        {
            get
            {
                var val = AppSettingUtils.Get("debug");
                bool result = false;
                Boolean.TryParse(val, out result);
                return result;
            }
        }
    }
}

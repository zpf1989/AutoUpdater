using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Components
{
    /// <summary>
    /// http请求json响应数据实体
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// 结果代码
        /// </summary>
        public ResultCode code { get; set; }
        /// <summary>
        /// 结果说明
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 结果数据
        /// </summary>
        public dynamic data { get; set; }

        public HttpResult()
        {
            code = ResultCode.Fail;
        }

        public static HttpResult Build(ResultCode code, string msg = "", dynamic data = null)
        {
            var rst = new HttpResult();
            rst.code = code;
            rst.msg = rst.code.GetDescription();
            if (!string.IsNullOrEmpty(msg))
            {
                rst.msg += "：" + msg;
            }
            rst.data = data;

            return rst;
        }
    }
}

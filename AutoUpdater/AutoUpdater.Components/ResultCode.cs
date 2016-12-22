using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Components
{
    public enum ResultCode
    {
        [Description("异常")]
        Exception = -1,
        [Description("成功")]
        Success = 1,
        [Description("失败")]
        Fail = 0,
        [Description("重复操作")]
        OptRepeat = 2,
        [Description("参数错误")]
        ParamError = 3,
        [Description("未找到token")]
        Tokenless = 4,
        [Description("token已失效")]
        TokenExpired = 5,
        [Description("非法token")]
        TokenIllegal = 6,
        [Description("已是最新版本")]
        NewestVersion = 7,
        [Description("有新版本")]
        NewVersion = 8,
    }
}

using AutoUpdater.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoUpdater.Host
{
    /// <summary>
    /// 消息处理程序：记录每次请求、响应内容
    /// </summary>
    public class CustomMessageHandler : DelegatingHandler
    {
        Logger<CustomMessageHandler> _logger = new Logger<CustomMessageHandler>();
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (HostContext.Debug)
            {
                string msg = string.Format("{0}本次请求:{0}{1}{0}", Environment.NewLine, request.ToString());

                if (request.Content != null)
                {
                    var content = request.Content.ReadAsStringAsync().Result;
                    msg += string.Format("Content:{0}\t{1}", Environment.NewLine, content);
                }
                _logger.LogInfo(msg);
            }

            return base.SendAsync(request, cancellationToken)
                     .ContinueWith(
                         (task) =>
                         {
                             //开启日志记录或异常时，都记录日志
                             if (HostContext.Debug || !task.Result.IsSuccessStatusCode)
                             {
                                 _logger.LogInfo(string.Format("本次响应：{0}Content：{1}", Environment.NewLine, task.Result.Content.ReadAsStringAsync().Result));
                             }
                             //如果异常了，重新设置task.Result.Content，屏蔽异常详细信息
                             if (!task.Result.IsSuccessStatusCode)
                             {
                                 task.Result.Content = new ObjectContent<object>(new HttpResult
                                 {
                                     code = ResultCode.Fail,
                                     msg = ((int)task.Result.StatusCode).ToString() + ":" + task.Result.ReasonPhrase
                                 }, HostContext.CurrentMediaTypeFormatter);
                             }
                             return task.Result;
                         }
                     );
        }
    }
}

using MyNet.Components.Compress;
using MyNet.Components.Logger;
using MyNet.Components.Result;
using MyNet.Components.Upgrade;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace AutoUpdater.Host.Api
{
    [RoutePrefix("api/client")]
    public class UpgradeApiController : ApiController
    {
        private const string Msg_CheckUpgrade = "检查新版本";
        private ILogHelper<UpgradeApiController> _logger = LogHelperFactory.GetLogHelper<UpgradeApiController>();
        private static IList<AppClient> _apps;
        private static string _zipFileTemp = HostContext.RootPath + "/Clients/{0}/upgrade.zip";

        public UpgradeApiController()
        {
            GetApps();
        }

        private void GetApps()
        {
            //获取客户端信息
            var data = File.ReadAllText(HostContext.RootPath + "app.json");
            _apps = JsonConvert.DeserializeObject<IList<AppClient>>(data);
        }

        [HttpGet]
        [Route("check")]
        public OptResult CheckUpgrade(string appid, string v2)
        {
            OptResult rst = null;
            var app = _apps.Where(a => a.appid == appid).FirstOrDefault();
            if (app == null)
            {
                rst = OptResult.Build(ResultCode.ParamError, Msg_CheckUpgrade + ",未识别的appid！");
                return rst;
            }
            Version _v2 = null;//客户端传递来的版本
            if (!Version.TryParse(v2, out _v2))
            {
                rst = OptResult.Build(ResultCode.ParamError, Msg_CheckUpgrade + ",非法版本格式！");
                return rst;
            }
            Version _v1 = null;//服务端设置的最新客户端版本
            if (!Version.TryParse(app.version, out _v1))
            {
                rst = OptResult.Build(ResultCode.ParamError, Msg_CheckUpgrade + ",非法版本格式！");
                return rst;
            }
            if (_v1 > _v2)
            {
                //有新版本
                rst = OptResult.Build(ResultCode.NewVersion, Msg_CheckUpgrade + ",有新版本", new { version = app.version });
                return rst;
            }
            rst = OptResult.Build(ResultCode.NewestVersion, Msg_CheckUpgrade + ",当前已是最新版本");
            return rst;
        }

        [HttpGet]
        [Route("upgrade")]
        public HttpResponseMessage Upgrade(string appid)
        {
            try
            {
                var app = _apps.Where(a => a.appid == appid).FirstOrDefault();
                if (app == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.NoContent);
                }
                var zipFile = string.Format(_zipFileTemp, appid);
                if (!File.Exists(zipFile))
                {
                    _logger.LogInfo(string.Format("正在制作升级包..."));
                    ZipCompresser.Compress(zipFile, string.Format(HostContext.RootPath + "Clients/{0}/Upgrade", appid));
                    _logger.LogInfo(string.Format("升级包制作完毕"));
                }
                else
                {
                    _logger.LogInfo(string.Format("升级包已存在，将直接下载"));
                }
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                FileStream fs = new FileStream(zipFile, FileMode.Open);
                response.Content = new StreamContent(fs);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = new FileInfo(zipFile).Name
                };
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}

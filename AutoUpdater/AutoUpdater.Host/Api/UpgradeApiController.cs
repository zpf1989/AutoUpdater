using AutoUpdater.Components;
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
        private Logger<UpgradeApiController> _logger = new Logger<UpgradeApiController>();
        private static ClientInfo _client;
        private static string _zipFile = HostContext.RootPath + "/Client/upgrade.zip";
        public UpgradeApiController()
        {
            GetNewstClient();
        }

        private void GetNewstClient()
        {
            //获取客户端信息
            var data = File.ReadAllText(HostContext.RootPath + "client.json");
            _client = JsonConvert.DeserializeObject<ClientInfo>(data);
        }

        [HttpGet]
        [Route("check")]
        public HttpResult CheckUpgrade(string v2)
        {
            HttpResult rst = null;
            Version _v2 = null;//客户端传递来的版本
            if (!Version.TryParse(v2, out _v2))
            {
                rst = HttpResult.Build(ResultCode.ParamError, Msg_CheckUpgrade + ",非法版本格式！");
                return rst;
            }
            Version _v1 = null;//服务端设置的最新客户端版本
            if (!Version.TryParse(_client.version, out _v1))
            {
                rst = HttpResult.Build(ResultCode.ParamError, Msg_CheckUpgrade + ",非法版本格式！");
                return rst;
            }
            if (_v1 > _v2)
            {
                //有新版本
                rst = HttpResult.Build(ResultCode.NewVersion, Msg_CheckUpgrade + ",有新版本", new { version = _client.version });
                return rst;
            }
            rst = HttpResult.Build(ResultCode.NewestVersion, Msg_CheckUpgrade + ",当前已是最新版本");
            return rst;
        }

        [HttpGet]
        [Route("upgrade")]
        public HttpResponseMessage Upgrade()
        {
            try
            {
                if (!File.Exists(_zipFile))
                {
                    _logger.LogInfo(string.Format("正在制作升级包..."));
                    ZipCompresser.Compress(_zipFile, HostContext.RootPath + "Client/Upgrade");
                    _logger.LogInfo(string.Format("升级包制作完毕"));
                }
                else
                {
                    _logger.LogInfo(string.Format("升级包已存在，将直接下载"));
                }
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                FileStream fs = new FileStream(_zipFile, FileMode.Open);
                response.Content = new StreamContent(fs);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = new FileInfo(_zipFile).Name
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

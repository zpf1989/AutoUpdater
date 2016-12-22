using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Components
{
    public class HttpUtils
    {
        static Logger<HttpUtils> _logger = new Logger<HttpUtils>();

        public static string Get(string url, string token = "")
        {
            HttpWebResponse response = null;
            try
            {
                var reqEncoding = Encoding.UTF8;
                var header = new HttpHeader();
                response = GetResponse(url: url, header: header, token: token);
                if (response == null)
                {
                    return string.Empty;
                }
                using (var stream = response.GetResponseStream())
                {
                    var bytes = GetBytes(stream);
                    var strResponse = reqEncoding.GetString(bytes, 0, bytes.Length);
                    //记录本次请求信息
                    _logger.LogInfo(string.Format("request:{0}\turl,{1}{0}response:{0}\t{2}",
                        Environment.NewLine, url, strResponse));
                    return strResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                if (response != null)
                {
                    response.Close();
                }

                throw new Exception(ex.Message, ex);
            }
        }

        public static HttpResult GetResult(string url, string token = "")
        {
            var str = Get(url, token);
            var rst = JsonConvert.DeserializeObject<HttpResult>(str);
            return rst;
        }

        public static string Post(string url, object jsonData, string token = "")
        {
            HttpWebResponse response = null;
            try
            {
                var reqEncoding = Encoding.UTF8;
                var header = new HttpHeader { contentType = "application/json;charset=utf-8", method = "POST" };
                response = GetResponse(url: url, header: header, token: token, jsonData: jsonData);
                using (var stream = response.GetResponseStream())
                {
                    var bytes = GetBytes(stream);
                    var strResponse = reqEncoding.GetString(bytes, 0, bytes.Length);
                    //记录本次请求信息
                    _logger.LogInfo(string.Format("request:{2}\turl,{0}{2}\tjsonData,{1}{2}response:{2}\t{3}",
                        url, JsonConvert.SerializeObject(jsonData), Environment.NewLine, strResponse));
                    return strResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                if (response != null)
                {
                    response.Close();
                }

                throw new Exception(ex.Message, ex);
            }
        }

        public static HttpResult PostResult(string url, object jsonData, string token = "")
        {
            var str = Post(url, jsonData, token);
            var rst = JsonConvert.DeserializeObject<HttpResult>(str);
            return rst;
        }

        private static HttpWebResponse GetResponse(string url, HttpHeader header,
            Encoding requestEncoding = null,
            int timeout = 100,
            string token = "",
            object jsonData = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (header == null)
            {
                header = new HttpHeader();
            }
            if (requestEncoding == null)
            {
                requestEncoding = Encoding.UTF8;
            }
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            try
            {
                //如果是发送HTTPS请求  
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((s, c, cf, err) => { return true; });
                    httpRequest = WebRequest.Create(url) as HttpWebRequest;
                    httpRequest.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    httpRequest = (HttpWebRequest)WebRequest.Create(url);
                }
                httpRequest.ContentType = header.contentType;
                httpRequest.ServicePoint.ConnectionLimit = header.maxTry;
                httpRequest.Referer = url;
                httpRequest.Accept = header.accept;
                httpRequest.UserAgent = header.userAgent;
                httpRequest.Method = header.method;

                httpRequest.Timeout = timeout * 1000;
                AddToken(httpRequest, token);

                if (jsonData != null)
                {
                    string jsonStr = JsonConvert.SerializeObject(jsonData);
                    byte[] data = requestEncoding.GetBytes(jsonStr);
                    using (Stream stream = httpRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                else
                {
                    httpRequest.ContentLength = 0;
                }

                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (Exception ex)
            {
                string err = "http请求错误：" + ex.Message;
                if (httpRequest != null)
                {
                    httpRequest.Abort();
                }
                if (httpResponse != null)
                {
                    httpResponse.Close();
                }
                _logger.LogError(ex);
                throw new Exception(err, ex);
            }
            return httpResponse;
        }


        private static void AddToken(HttpWebRequest request, string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }
            request.Headers.Add("token", token);
        }

        private static byte[] GetBytes(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static void Download(string url, string filename, Action<DownloadProgressChangedEventArgs> downloadProgressChangeCallback = null,
            Action<AsyncCompletedEventArgs> downloadCompleteCallback = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }
            WebClient client = new WebClient();
            try
            {
                client.DownloadProgressChanged += (o, e) =>
                {
                    downloadProgressChangeCallback?.Invoke(e);
                };
                client.DownloadFileCompleted += (o, e) =>
                {
                    if (e.Error == null)
                    {
                        _logger.LogInfo(string.Format("文件下载成功，from\t{0}\tto\t{1}", url, filename));
                    }
                    else
                    {
                        _logger.LogError(string.Format("文件下载失败，from\t{0}\tto\t{1}", url, filename), e.Error);
                    }
                    downloadCompleteCallback?.Invoke(e);
                };
                var dir = new FileInfo(filename).Directory;
                if (!dir.Exists)
                {
                    dir.Create();
                }
                client.DownloadFileAsync(new Uri(url), filename);
            }
            catch (Exception ex)
            {
                var err = string.Format("下载文件出错：{0}", ex.Message);
                client.Dispose();
                _logger.LogError(err, ex);
                throw new Exception(err, ex);
            }
        }
    }
}

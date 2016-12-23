
using MyNet.Components.Logger;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace AutoUpdater.Host
{
    public class Startup
    {
        static HttpConfiguration _httpConfig;
        static ILogHelper<Startup> _logger = LogHelperFactory.GetLogHelper<Startup>();

        public void Configuration(IAppBuilder appBuilder)
        {
            _httpConfig = new HttpConfiguration();

            //1、自定义路由
            _httpConfig.MapHttpAttributeRoutes();//映射RouteAttribute属性
            _httpConfig.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}"
                );

            //2、序列化器
            var jsonFormatter = _httpConfig.Formatters.JsonFormatter;
            //解决json序列化时的循环引用问题
            jsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            HostContext.CurrentMediaTypeFormatter = jsonFormatter;
            //干掉xml序列化器
            _httpConfig.Formatters.Remove(_httpConfig.Formatters.XmlFormatter);

            //3、自定义消息处理程序
            _httpConfig.MessageHandlers.Add(new CustomMessageHandler());

            appBuilder.UseWebApi(_httpConfig);
        }
    }
}

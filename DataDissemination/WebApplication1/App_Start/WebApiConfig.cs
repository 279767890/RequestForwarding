using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using static WebApplication1.Controllers.InvoicesController;

namespace WebApplication1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "InvoiceImplementation/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            var jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
            config.Services.Replace(typeof(System.Net.Http.Formatting.IContentNegotiator), new JsonContentNegotiator(jsonFormatter));
        }
    }
}

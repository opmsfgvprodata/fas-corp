using MVC_SYSTEM.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MVC_SYSTEM
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            config.MapHttpAttributeRoutes();
            //var cors = new EnableCorsAttribute("*", "*", "*");

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        public static string GetUrlMobile()
        {
            string getresult = "";
            IniParser parser = new IniParser(AppDomain.CurrentDomain.BaseDirectory + "config.ini");

            getresult = parser.GetSetting("configdeclaration", "mobileapiurl");

            return getresult;
        }
    }
}

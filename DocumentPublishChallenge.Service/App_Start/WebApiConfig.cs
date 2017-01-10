using System.Web.Http;
using System.Web.Http.Cors;
using DocumentPublishChallenge.DataAccessLayer;
using DocumentPublishChallenge.Service.Code;
using Microsoft.Practices.Unity;
using static System.Configuration.ConfigurationManager;

namespace DocumentPublishChallenge.Service
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // https://www.asp.net/web-api/overview/security/enabling-cross-origin-requests-in-web-api
            var cors = new EnableCorsAttribute(AppSettings["DocumentPublishWebUIUrl"], "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "API Default",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });

            // https://www.asp.net/web-api/overview/advanced/dependency-injection
            var container = new UnityContainer();
            container.RegisterType<IDocumentRepository, DocumentContext>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);
        }
    }
}
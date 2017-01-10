using System.Web.Mvc;
using System.Web.Routing;
using TaskList.Constraints;

namespace TaskList
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "TODO",
               url: "{controller}/TODO",
               defaults: new { controller = "Task", action = "AllTasks" }
           );
            routes.MapRoute(
                name: "task",
                 url: "{controller}/{id}",
                 defaults: new { controller = "Task", action = "EditTask"},
                 constraints:new { id = new IdConstraint() }
                );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Authorization", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}
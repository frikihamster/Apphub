using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Orchard.CulturePicker {
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                //TO DO (ermakovich): still not sure why we need it, but without this route Orchard can`t find controller action properly
                new RouteDescriptor {
                    Route = new Route(
                        "ChangeCulture",
                        new RouteValueDictionary {
                            {"area", "Orchard.CulturePicker"},
                            {"controller", "UserCulture"},
                            {"action", "ChangeCulture"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Orchard.CulturePicker"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}
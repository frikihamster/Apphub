using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Orchard.CulturePicker {
    [OrchardFeature("Orchard.CulturePicker.HomePageRedirect")]
    public class HomeRoutes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Priority = 21,
                    Route = new Route(
                        "",
                        new RouteValueDictionary {
                            {"area", "Orchard.CulturePicker"},
                            {"controller", "LocalizableHome"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary {
                            {"area", "Orchard.CulturePicker"},
                            {"controller", "LocalizableHome"},
                        },
                        new RouteValueDictionary {
                            {"area", "Orchard.CulturePicker"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Core.Containers;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Orchard.CulturePicker {
    [OrchardFeature("Orchard.CulturePicker.Containers")]
    public class ContainersRoutes : IRouteProvider {
        private readonly IContainersPathConstraint _containersPathConstraint;

        public ContainersRoutes(IContainersPathConstraint containersPathConstraint) {
            _containersPathConstraint = containersPathConstraint;
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Priority = 16,
                    Route = new Route(
                        "{*path}",
                        new RouteValueDictionary {
                            {"area", "Orchard.CulturePicker"},
                            {"controller", "LocalizableItem"},
                            {"action", "Display"}
                        },
                        new RouteValueDictionary {
                            {"path", _containersPathConstraint}
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
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace BinaryAnalysis.MultiLanguage
{
    [OrchardFeature("BinaryAnalysis.CulturePicker")]
    public class PickerRoutes : IRouteProvider
    {
        #region Implementation of IRouteProvider

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Route = new Route(
                        "ChangeCulture",
                        new RouteValueDictionary {
                            {"area", "BinaryAnalysis.MultiLanguage"},
                            {"controller", "Culture"},
                            {"action", "ChangeCulture"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "BinaryAnalysis.MultiLanguage"}
                        },
                        new MvcRouteHandler())
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        #endregion
    }
}

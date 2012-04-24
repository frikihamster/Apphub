using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BinaryAnalysis.MultiLanguage.Services;
using Orchard;
using Orchard.Alias.Implementation.Holder;
using Orchard.Alias.Implementation.Map;
using Orchard.Autoroute.Models;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Mvc.Routes;
using Orchard.Settings;

namespace BinaryAnalysis.MultiLanguage
{
    [OrchardFeature("BinaryAnalysis.LocalizedHome")]
    public class HomeRoutes : IRouteProvider
    {
        private readonly IAliasHolder _aliasHolder;
        private readonly IWorkContextAccessor _workContextAccessor;

        public HomeRoutes(IAliasHolder aliasHolder,
            IWorkContextAccessor workContextAccessor)
        {
            _aliasHolder = aliasHolder;
            _workContextAccessor = workContextAccessor;
        }

        #region Implementation of IRouteProvider

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Priority = 81,
                    Route = new HomeRoute(_aliasHolder, _workContextAccessor )
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

    public class HomeRoute : RouteBase
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly AliasMap _aliasMap;
        private const string AreaName = "Contents";

        public HomeRoute(IAliasHolder aliasHolder, IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
            _aliasMap = aliasHolder.GetMap(AreaName);
        }

        #region Overrides of RouteBase


        public override RouteData GetRouteData(HttpContextBase httpContext) {
            // Get the full inbound request path
            var virtualPath = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;
            if (!String.IsNullOrWhiteSpace(virtualPath)) return null;

            //string siteCulture = _siteService.GetSiteSettings().SiteCulture;
            var cultureCookie = httpContext.Request.Cookies[CookieCultureSelector.CultureCookieName];
            string currentCulture = cultureCookie == null ? "" : cultureCookie[CookieCultureSelector.CurrentCultureFieldName];
            if (String.IsNullOrWhiteSpace(currentCulture)) return null;

            // Attempt to lookup RouteValues in the alias map
            IDictionary<string, string> routeValues;
            // TODO: Might as well have the lookup in AliasHolder...
            if (!_aliasMap.TryGetAlias(virtualPath, out routeValues)) return null;

            using (var scope = _workContextAccessor.CreateWorkContextScope()) {
                var _contentManager = scope.Resolve<IContentManager>();
                var _localizationService = _workContextAccessor.GetContext(httpContext).Resolve<ILocalizationService>();

                var id = Int32.Parse(routeValues["Id"]);
                var currentLocPart = _contentManager.Query<LocalizationPart, LocalizationPartRecord>()
                    .Where(part => part.ContentItemRecord.Id == id)
                    .List().FirstOrDefault();

                if (currentLocPart != null) {
                    var locPart = _localizationService.GetLocalizations(currentLocPart.ContentItem, VersionOptions.Latest)
                        .Where(l => l.Culture.Culture == currentCulture)
                        .FirstOrDefault();
                    if (locPart != null) {
                        var newRouteRecord = _contentManager.Query<AutoroutePart, AutoroutePartRecord>()
                            .Where(part => part.ContentItemRecord.Id == locPart.ContentItem.Id)
                            .List().FirstOrDefault();
                        if (newRouteRecord != null) {
                            routeValues["Id"] = newRouteRecord.ContentItem.Id.ToString();
                        }
                    }
                }

                // Construct RouteData from the route values
                var data = new RouteData(this, new MvcRouteHandler());
                foreach (var routeValue in routeValues) {
                    var key = routeValue.Key;
                    if (key.EndsWith("-"))
                        data.Values.Add(key.Substring(0, key.Length - 1), routeValue.Value);
                    else
                        data.Values.Add(key, routeValue.Value);
                }

                data.Values["area"] = AreaName;
                data.DataTokens["area"] = AreaName;

                return data;
            }
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values) {
            return null;
        }

        #endregion
    }



}

using System;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Routable.Models;
using Orchard.Localization.Models;
using Orchard.Localization.Services;

namespace Orchard.CulturePicker.Services {
    public class LocalizableContentService : ILocalizableContentService {
        private readonly ILocalizationService _localizationService;
        private readonly ICultureManager _cultureManager;
        private readonly IContentManager _contentManager;

        public LocalizableContentService(ILocalizationService localizationService, ICultureManager cultureManager, IContentManager contentManager) {
            _localizationService = localizationService;
            _cultureManager = cultureManager;
            _contentManager = contentManager;        
        }

        //Finds route part for the specified URL
        //Returns true if specified url corresponds to some content and route exists; otherwise - false
        public bool TryGetRouteForUrl(string url, out RoutePart route) {
            //first check for route (fast, case sensitive, not precise)
            route = _contentManager.Query<RoutePart, RoutePartRecord>()
                .ForVersion(VersionOptions.Published)
                .Where(r => r.Path == url)
                .List()
                .FirstOrDefault();
            
            return route != null;
        }

        //Finds localized route part for the specified content and culture
        //Returns true if localized url for content and culture exists; otherwise - false
        public bool TryFindLocalizedRoute(ContentItem routableContent, string cultureName, out RoutePart localizedRoute) {
            if (!routableContent.Parts.Any(p => p.Is<LocalizationPart>())) {
                localizedRoute = null;
                return false;
            }

            var siteCulture = _cultureManager.GetCultureByName(_cultureManager.GetSiteCulture());
            var localizations = _localizationService.GetAvailableLocalizations(routableContent, VersionOptions.Published, siteCulture);
            var localizationPart = localizations.FirstOrDefault(l => l.Culture.Culture == cultureName);

            //try get localization part for default site culture
            if (localizationPart == null) {
                localizationPart = localizations.FirstOrDefault(l => l.Culture.Equals(siteCulture));
            }

            if (localizationPart == null) {
                localizedRoute = null;
                return false;
            }

            var localizedContentItem = localizationPart.ContentItem;
            localizedRoute = localizedContentItem.Parts.Single(p => p is RoutePart).As<RoutePart>();
            return true;
        }
    }
}
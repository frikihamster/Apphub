using System.Web.UI.WebControls;
using Orchard.ContentManagement;
using Orchard.Core.Routable.Models;

namespace Orchard.CulturePicker.Services {
    public interface ILocalizableContentService : IDependency {
        bool TryFindLocalizedRoute(ContentItem routableContent, string cultureName, out RoutePart localizedRoute);
        bool TryGetRouteForUrl(string url, out RoutePart route);
    }
}
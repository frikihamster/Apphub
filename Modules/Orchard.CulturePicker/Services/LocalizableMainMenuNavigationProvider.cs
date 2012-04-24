using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.Core.Navigation.Models;
using Orchard.Localization;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using System.Web;
using Orchard.Environment.Extensions;

namespace Orchard.CulturePicker.Services {
    [UsedImplicitly]
    [OrchardFeature("Orchard.CulturePicker.MainMenu")]
    [OrchardSuppressDependencyAttribute("Orchard.Core.Navigation.Services.MainMenuNavigationProvider")]
    public class LocalizableMainMenuNavigationProvider : INavigationProvider {
        private readonly IContentManager _contentManager;
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public LocalizableMainMenuNavigationProvider(IContentManager contentManager, ICultureManager cultureManager, IWorkContextAccessor workContextAccessor) {
            _contentManager = contentManager;
            _cultureManager = cultureManager;
            _workContextAccessor = workContextAccessor;
        }

        public string MenuName { get { return "main"; } }

        public void GetNavigation(NavigationBuilder builder) {
            IEnumerable<MenuPart> menuParts;

            if(AdminFilter.IsApplied(_workContextAccessor.GetContext().HttpContext.Request.RequestContext))
                menuParts = _contentManager.Query<MenuPart, MenuPartRecord>().Where(x => x.OnMainMenu).List();
            else
                menuParts = FilterForCurrentCulture(_contentManager.Query<MenuPart, MenuPartRecord>().Where(x => x.OnMainMenu).List());

            foreach (var menuPart in menuParts) {
                if (menuPart != null)
                {
                    var part = menuPart;

                    if (part.Is<MenuItemPart>())
                        builder.Add(new LocalizedString(HttpUtility.HtmlEncode(part.MenuText)), part.MenuPosition, item => item.Url(part.As<MenuItemPart>().Url));
                    else
                        builder.Add(new LocalizedString(HttpUtility.HtmlEncode(part.MenuText)), part.MenuPosition, item => item.Action(_contentManager.GetItemMetadata(part.ContentItem).DisplayRouteValues));
                }
            }
        }

        #region Helpers

        /// Filters menu items for current culture
        private IEnumerable<MenuPart> FilterForCurrentCulture(IEnumerable<MenuPart> items) {
            var currentCulture = _cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext);

            foreach (var item in items) {
                var localizationPart = item.ContentItem.Parts.FirstOrDefault(p => p is LocalizationPart).As<LocalizationPart>();

                if (localizationPart == null || localizationPart.Culture == null) continue;

                //TODO (ermakovich): need to take into account menu items without translations. Would be nice to display them as well.
                if (localizationPart.Culture.Culture == currentCulture)
                    yield return item;
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Core.Routable.Models;
using Orchard.CulturePicker.Services;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Logging;
using Orchard.Mvc.Extensions;
using Orchard.Services;
using Orchard.Themes;

namespace Orchard.CulturePicker.Controllers {
    [HandleError]
    [OrchardFeature("Orchard.CulturePicker.HomePageRedirect")]
    public class LocalizableHomeController : Controller {
        private readonly IEnumerable<IHomePageProvider> _homePageProviders;
        private readonly IOrchardServices _orchardServices;
        private readonly IContentManager _contentManager;
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ILocalizableContentService _localizableContentService;

        public LocalizableHomeController(IEnumerable<IHomePageProvider> homePageProviders, IOrchardServices orchardServices, IContentManager contentManager, ICultureManager cultureManager, IWorkContextAccessor workContextAccessor, ILocalizableContentService localizableContentService)
        {
            _homePageProviders = homePageProviders;
            _orchardServices = orchardServices;
            _contentManager = contentManager;
            _cultureManager = cultureManager;
            _workContextAccessor = workContextAccessor;
            _localizableContentService = localizableContentService;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [Themed]
        public ActionResult Index() {
            try {
                var homepage = _orchardServices.WorkContext.CurrentSite.HomePage;
                if (String.IsNullOrEmpty(homepage))
                    return View();

                var homePageParameters = homepage.Split(';');
                if (homePageParameters.Length != 2)
                    return View();

                var providerName = homePageParameters[0];
                var routeId = Int32.Parse(homePageParameters[1]);

                var currentRoutePart = _contentManager.Get(routeId, VersionOptions.Published);
                if (currentRoutePart == null || !currentRoutePart.Is<RoutePart>())
                    return new HttpNotFoundResult();

                var currentCultureName = _cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext);
                RoutePart localizedRoutePart;
                //content may not have localized version and we use "Try" approach
                if (_localizableContentService.TryFindLocalizedRoute(currentRoutePart.As<RoutePart>().ContentItem, currentCultureName, out localizedRoutePart)) {
                    return this.RedirectLocal(localizedRoutePart.Path, (Func<ActionResult>)null);
                }

                foreach (var provider in _homePageProviders) {
                    if (!string.Equals(provider.GetProviderName(), providerName))
                        continue;

                    var result = provider.GetHomePage(routeId);
                    if (result is ViewResultBase) {
                        var resultBase = result as ViewResultBase;
                        ViewData.Model = resultBase.ViewData.Model;
                        resultBase.ViewData = ViewData;
                    }

                    return result;
                }

                return View();
            }
            catch {
                return View();
            }
        }
    }

}
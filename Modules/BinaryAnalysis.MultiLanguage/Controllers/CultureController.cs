using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BinaryAnalysis.MultiLanguage.Services;
using Orchard;
using Orchard.Alias;
using Orchard.Alias.Implementation.Holder;
using Orchard.Autoroute.Models;
using Orchard.Autoroute.Services;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Mvc.Extensions;

namespace BinaryAnalysis.MultiLanguage.Controllers
{
    public class CultureController : Controller 
    {
        public IOrchardServices Services { get; set; }
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IContentManager _contentManager;
        public Localizer T { get; set; }

        public CultureController(IOrchardServices services, IWorkContextAccessor workContextAccessor,
            ILocalizationService localizationService, IContentManager contentManager)
        {
            Services = services;
            _workContextAccessor = workContextAccessor;
            _localizationService = localizationService;
            _contentManager = contentManager;
        }

        public ActionResult ChangeCulture(string cultureName) {
            if (string.IsNullOrEmpty(cultureName)) throw new ArgumentNullException(cultureName);

            var returnUrl = GetReturnUrl(_workContextAccessor.GetContext().HttpContext);

            var routeRecord = _contentManager.Query<AutoroutePart, AutoroutePartRecord>()
                .Where(part => part.DisplayAlias != null && part.DisplayAlias == returnUrl)
                .List().FirstOrDefault();

            if (routeRecord != null) {
                var currentLocPart = routeRecord.ContentItem.Parts.OfType<LocalizationPart>().SingleOrDefault();
                if (currentLocPart != null)
                {
                    var locPart = _localizationService.GetLocalizations(currentLocPart.ContentItem, VersionOptions.Latest)
                    .Where(l => l.Culture.Culture == cultureName)
                    .FirstOrDefault();
                    if (locPart != null) {
                        var newRouteRecord = _contentManager.Query<AutoroutePart, AutoroutePartRecord>()
                            .Where(part => part.ContentItemRecord.Id == locPart.ContentItem.Id)
                            .List().FirstOrDefault();
                        if (newRouteRecord != null) {
                            returnUrl = newRouteRecord.DisplayAlias;
                        }
                    }
                }

                //routeRecord.ContentItem
            }

            SaveCultureCookie(cultureName);
            return this.RedirectLocal(returnUrl.StartsWith("/") ? returnUrl : "/" + returnUrl);
        }

        #region Helpers

        //Saves culture information to cookie
        private void SaveCultureCookie(string cultureName) {
            var cultureCookie = new HttpCookie(CookieCultureSelector.CultureCookieName);
            cultureCookie.Values.Add(CookieCultureSelector.CurrentCultureFieldName, cultureName);
            cultureCookie.Expires = DateTime.Now.AddYears(1);
            Services.WorkContext.HttpContext.Response.Cookies.Add(cultureCookie);
        }

        //returns url based on the current http context
        //takes into account, that url can contain application path
        private static string GetReturnUrl(HttpContextBase context) {
            var absolutePath = context.Request.UrlReferrer.AbsolutePath;
            return absolutePath.Remove(0, context.Request.ApplicationPath.Length).Trim('\\', '/');
        }

            #endregion
        }
}

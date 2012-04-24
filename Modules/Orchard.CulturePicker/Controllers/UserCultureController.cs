using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Core.Routable.Models;
using Orchard.CulturePicker.Services;
using Orchard.Localization;
using Orchard.Mvc.Extensions;

namespace Orchard.CulturePicker.Controllers {
    public class UserCultureController : Controller {
        public IOrchardServices Services { get; set; }
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ILocalizableContentService _localizableContentService;
        public Localizer T { get; set; }

        public UserCultureController(IOrchardServices services, IWorkContextAccessor workContextAccessor, ILocalizableContentService localizableContentService) {
            Services = services;
            _workContextAccessor = workContextAccessor;
            _localizableContentService = localizableContentService;
        }

        public ActionResult ChangeCulture(string cultureName) {
            if (string.IsNullOrEmpty(cultureName)) throw new ArgumentNullException(cultureName);

            var returnUrl = GetReturnUrl(_workContextAccessor.GetContext().HttpContext);

            RoutePart currentRoutePart;
            //returnUrl may not correspond to any content and we use "Try" approach
            if (_localizableContentService.TryGetRouteForUrl(returnUrl, out currentRoutePart)) {
                RoutePart localizedRoutePart;
                //content may not have localized version and we use "Try" approach
                if (_localizableContentService.TryFindLocalizedRoute(currentRoutePart.ContentItem, cultureName, out localizedRoutePart))
                    returnUrl = localizedRoutePart.Path;
            }

            SaveCultureCookie(cultureName);
            return this.RedirectLocal(returnUrl);
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
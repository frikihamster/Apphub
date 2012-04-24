using System;
using System.Web;
using Orchard.Localization.Services;

namespace Orchard.CulturePicker.Services {
    public class CookieCultureSelector: ICultureSelector {

        public const string CultureCookieName = "cultureData";
        public const string CurrentCultureFieldName = "currentCulture";
        public const int SelectorPriority = -3; //priority is higher than SiteCultureSelector priority (-5)

        public CultureSelectorResult GetCulture(HttpContextBase context) {
            if (context == null || context.Request == null || context.Request.Cookies == null) return null;

            var cultureCookie = context.Request.Cookies[CultureCookieName];

            if (cultureCookie == null) return null;

            var currentCultureName = cultureCookie[CurrentCultureFieldName];
            return String.IsNullOrEmpty(currentCultureName) ? null : new CultureSelectorResult {Priority = SelectorPriority, CultureName = currentCultureName};
        }
    }
}
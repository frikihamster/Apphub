using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization.Models;
using Orchard.Localization.Records;
using Orchard.Localization.Services;

namespace Orchard.CulturePicker.Services {
    public static class LocalizationServiceExtensions {
        public static IEnumerable<LocalizationPart> GetAvailableLocalizations(this ILocalizationService localizationService, IContent content, VersionOptions versionOptions, CultureRecord siteCulture) {
            return localizationService.GetLocalizations(content.ContentItem, versionOptions)
                .Select(c =>
                {
                    var localized = c.ContentItem.As<LocalizationPart>();
                    if (localized.Culture == null)
                        localized.Culture = siteCulture;
                    return c;
                }).ToList();
        }
    }
}
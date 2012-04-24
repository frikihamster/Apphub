using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Common.Drivers;
using Orchard.Core.Common.Models;
using Orchard.Core.Common.Settings;
using Orchard.Environment.Extensions;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Services;

namespace BinaryAnalysis.MultiLanguage.Drivers {
    [UsedImplicitly]
    [OrchardFeature("BinaryAnalysis.CultureFallback")]
    [OrchardSuppressDependency("Orchard.Core.Common.Drivers.BodyPartDriver")]
    public class CultureBodyPartDriver : BodyPartDriver 
    {
        private readonly ILocalizationService _locService;
        private readonly IOrchardServices _services;
        private readonly ICultureManager _cutureManager;
        private readonly IEnumerable<IHtmlFilter> _htmlFilters;

        public CultureBodyPartDriver(
            ILocalizationService locService,
            IOrchardServices services, 
            ICultureManager cutureManager,
            RequestContext requestContext,
            IEnumerable<IHtmlFilter> htmlFilters)
            : base(services, htmlFilters, requestContext)
        {
            _locService = locService;
            _services = services;
            _cutureManager = cutureManager;
            _htmlFilters = htmlFilters;
        }

        protected override DriverResult Display(BodyPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_Common_Body",
                             () => {
                                 var bodyText = GetFallbackBodyText(part);
                                 return shapeHelper.Parts_Common_Body(ContentPart: part, Html: new HtmlString(bodyText));
                             }),
                ContentShape("Parts_Common_Body_Summary",
                             () => {
                                 var bodyText = GetBodyText(part);
                                 return shapeHelper.Parts_Common_Body_Summary(ContentPart: part, Html: new HtmlString(bodyText));
                             })
                );
        }
        private string GetFallbackBodyText(BodyPart part) {

            var locPart = part.ContentItem.Parts.OfType<LocalizationPart>().SingleOrDefault();
            if(locPart!=null) {

                var currentCulture = locPart.Culture.Culture;//_services.WorkContext.CurrentCulture;
                var siteCulture = _cutureManager.GetSiteCulture();
                var partText = GetBodyText(part);

                if (string.IsNullOrWhiteSpace(partText) && siteCulture != currentCulture) {

                    var master = locPart.MasterContentItem.ContentItem.Parts.OfType<BodyPart>().SingleOrDefault();
                    return GetBodyText(master);
                }
            }
            return GetBodyText(part);
        }
        private string GetBodyText(BodyPart part) 
        {
            return _htmlFilters.Aggregate(part.Text, (text, filter) => filter.ProcessContent(text, GetFlavor(part)));
        }

        private static string GetFlavor(BodyPart part)
        {
            var typePartSettings = part.Settings.GetModel<BodyTypePartSettings>();
            return (typePartSettings != null && !string.IsNullOrWhiteSpace(typePartSettings.Flavor))
                       ? typePartSettings.Flavor
                       : part.PartDefinition.Settings.GetModel<BodyPartSettings>().FlavorDefault;
        }
    }
}
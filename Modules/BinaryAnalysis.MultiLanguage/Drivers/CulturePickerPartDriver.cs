using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryAnalysis.MultiLanguage.Models;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;

namespace BinaryAnalysis.MultiLanguage.Drivers
{
    [UsedImplicitly]
    [OrchardFeature("BinaryAnalysis.CulturePicker")]
    public class CulturePickerPartDriver : ContentPartDriver<CulturePickerPart>
    {
        private readonly ICultureManager _cultureManager;
        private readonly IWorkContextAccessor _workContextAccessor;

        public CulturePickerPartDriver(ICultureManager cultureManager, IWorkContextAccessor workContextAccessor)
        {
            _cultureManager = cultureManager;
            _workContextAccessor = workContextAccessor;
        }

        protected override DriverResult Display(CulturePickerPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_CulturePicker", () => {
                part.AvailableCultures = _cultureManager.ListCultures();
                part.UserCulture = _cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext);
                return shapeHelper.Parts_CulturePicker(AvailableCultures: part.AvailableCultures, UserCulture: part.UserCulture);
            });
        }
    }
}

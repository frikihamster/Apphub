using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.ContentManagement;

namespace BinaryAnalysis.MultiLanguage.Models
{
    public class CulturePickerPart : ContentPart
    {
        public IEnumerable<string> AvailableCultures { get; set; }

        public string UserCulture { get; set; }
    }
}

//using Orchard.Localization;
//using Orchard.UI.Navigation;

//namespace BinaryAnalysis.MultiLanguage
//{
//    public class AdminMenu : INavigationProvider
//    {
//        public Localizer T { get; set; }

//        public string MenuName { get { return "admin"; } }

//        public void GetNavigation(NavigationBuilder builder)
//        {
//            builder.AddImageSet("culture")
//                .Add(T("Culture"), "6",
//                    menu => menu.Add(T("Index"), "0", item => item.Action("Index", "AdminCulture", new { area = "BinaryAnalysis.MultiLanguage" })
//                        ));
//        }
//    }
//}

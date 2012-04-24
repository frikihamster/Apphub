﻿using System;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers;
using Orchard.Core.Containers.Extensions;
using Orchard.Core.Containers.Models;
using Orchard.Core.Routable.Models;
using Orchard.DisplayManagement;
using Orchard.Localization.Models;
using Orchard.Localization.Services;
using Orchard.Themes;
using Orchard.UI.Navigation;
using Orchard.Settings;

namespace Orchard.CulturePicker.Controllers {

    public class LocalizableItemController : Controller {
        private readonly IContentManager _contentManager;
        private readonly IContainersPathConstraint _containersPathConstraint;
        private readonly ISiteService _siteService;
        private readonly ICultureManager _cultureManager;

        public LocalizableItemController(
            IContentManager contentManager, 
            IContainersPathConstraint containersPathConstraint, 
            IShapeFactory shapeFactory,
            ISiteService siteService,
            ICultureManager cultureManager) {

            _contentManager = contentManager;
            _containersPathConstraint = containersPathConstraint;
            _siteService = siteService;
            _cultureManager = cultureManager;
            Shape = shapeFactory;
        }

        dynamic Shape { get; set; }

        [Themed]
        public ActionResult Display(string path, PagerParameters pagerParameters) {
            var matchedPath = _containersPathConstraint.FindPath(path);
            if (string.IsNullOrEmpty(matchedPath)) {
                throw new ApplicationException("404 - should not have passed path constraint");
            }

            var hits = _contentManager
                .Query<RoutePart, RoutePartRecord>(VersionOptions.Published)
                .Where(r => r.Path == matchedPath)
                .Slice(0, 2);

            if (hits.Count() == 0) {
                throw new ApplicationException("404 - should not have passed path constraint");
            }

            if (hits.Count() != 1) {
                throw new ApplicationException("Ambiguous content");
            }

            var container = _contentManager.Get(hits.Single().Id);
            var cultureRecord =  _cultureManager.GetCultureByName(_cultureManager.GetCurrentCulture(HttpContext));
            IContentQuery<ContentItem> query = _contentManager
                .Query(VersionOptions.Published)
                .Join<CommonPartRecord>().Where(cr => cr.Container.Id == container.Id).Join<LocalizationPartRecord>().Where(lr => lr.CultureId == cultureRecord.Id);

            var descendingOrder = container.As<ContainerPart>().Record.OrderByDirection == (int) OrderByDirection.Descending;
            query = query.OrderBy(container.As<ContainerPart>().Record.OrderByProperty, descendingOrder);

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            pager.PageSize = pagerParameters.PageSize != null && container.As<ContainerPart>().Record.Paginated
                               ? pager.PageSize
                               : container.As<ContainerPart>().Record.PageSize;
            var pagerShape = Shape.Pager(pager).TotalItemCount(query.Count());

            var startIndex = container.As<ContainerPart>().Record.Paginated ? pager.GetStartIndex() : 0;
            var pageOfItems = query.Slice(startIndex, pager.PageSize).ToList();

            var list = Shape.List();
            list.AddRange(pageOfItems.Select(item => _contentManager.BuildDisplay(item, "Summary")));

            dynamic viewModel = Shape.ViewModel()
                .ContentItems(list)
                .Pager(pagerShape)
                .ShowPager(container.As<ContainerPart>().Record.Paginated);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object)viewModel);
        }
    }
}
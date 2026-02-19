using Contensive.Addons.Tools.Models.Db;
using Contensive.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models.View {
    /// <summary>
    /// View model for the Site Map display
    /// </summary>
    public class SiteMapViewModel {
        public bool hasPages { get; set; }
        public List<SiteMapPageViewModel> pages { get; set; }

        /// <summary>
        /// Create the site map view model from the database.
        /// Pages are flattened into a depth-ordered list for Mustache rendering.
        /// </summary>
        public static SiteMapViewModel create(CPBaseClass cp) {
            try {
                var result = new SiteMapViewModel {
                    pages = new List<SiteMapPageViewModel>()
                };

                // Load all active pages
                var allPages = PageContentModel.createList<PageContentModel>(cp, "(active<>0)", "name");
                if (allPages == null || !allPages.Any()) {
                    result.hasPages = false;
                    return result;
                }

                result.hasPages = true;

                // Organize pages by parent
                var pagesByParent = new Dictionary<int, List<PageContentModel>>();
                foreach (var page in allPages) {
                    int parentId = page.parentId;
                    if (!pagesByParent.ContainsKey(parentId)) {
                        pagesByParent[parentId] = new List<PageContentModel>();
                    }
                    pagesByParent[parentId].Add(page);
                }

                // Flatten the tree into a depth-ordered list starting from root pages (parentId = 0)
                flattenPageTree(cp, pagesByParent, 0, 0, result.pages);

                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return new SiteMapViewModel { hasPages = false };
            }
        }

        /// <summary>
        /// Recursively flatten the page tree into a depth-ordered list
        /// </summary>
        private static void flattenPageTree(CPBaseClass cp, Dictionary<int, List<PageContentModel>> pagesByParent, int parentId, int depth, List<SiteMapPageViewModel> flatList) {
            if (!pagesByParent.ContainsKey(parentId)) {
                return;
            }

            foreach (var page in pagesByParent[parentId]) {
                flatList.Add(new SiteMapPageViewModel {
                    id = page.id,
                    name = cp.Utils.EncodeText(page.name),
                    paddingLeft = depth * 20,
                    isRoot = depth == 0,
                    siteUrl = cp.Content.GetPageLink(page.id),
                    editUrl = cp.Content.GetEditUrl("page content", page.id),
                    pageHits = page.viewings.ToString(),
                    pageLastModifiedDate = page.modifiedDate is null ? "" : ((DateTime)page.modifiedDate).ToString("yyyy-MM-dd")
                });

                // Recurse into children
                if (pagesByParent.ContainsKey(page.id)) {
                    flattenPageTree(cp, pagesByParent, page.id, depth + 1, flatList);
                }
            }
        }
    }

    /// <summary>
    /// View model for individual page in the site map
    /// </summary>
    public class SiteMapPageViewModel {
        public int id { get; set; }
        public string name { get; set; }
        public int paddingLeft { get; set; }
        public bool isRoot { get; set; }
        public string siteUrl { get; set; }
        public string editUrl { get; set; }
        public string pageHits { get; set; }
        public string pageLastModifiedDate { get; set; }
    }
}

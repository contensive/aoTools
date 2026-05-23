using Contensive.Addons.Tools.Models.Db;
using Contensive.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contensive.Addons.Tools.Models.View {
    /// <summary>
    /// View model for the Page Review card display
    /// </summary>
    public class PageReviewViewModel {
        public bool hasPages { get; set; }
        public List<PageReviewPageViewModel> pages { get; set; }

        /// <summary>
        /// Create the page review view model from the database.
        /// Pages are sorted by review age descending (oldest reviews first).
        /// </summary>
        public static PageReviewViewModel create(CPBaseClass cp) {
            try {
                var result = new PageReviewViewModel {
                    pages = new List<PageReviewPageViewModel>()
                };
                //
                // -- load all pages
                var allPages = PageContentModel.createList<PageContentModel>(cp, "");
                if (allPages == null || !allPages.Any()) {
                    result.hasPages = false;
                    return result;
                }
                //
                result.hasPages = true;
                int pageContentId = cp.Content.GetID(PageContentModel.tableMetadata.contentName);
                //
                // -- build view models with review age
                var unsortedPages = new List<PageReviewPageViewModel>();
                foreach (var page in allPages) {
                    DateTime dateAdded = (page.dateAdded == null) ? DateTime.Now : (DateTime)page.dateAdded;
                    DateTime dateReviewed = (page.dateReviewed == null) ? dateAdded : (DateTime)page.dateReviewed;
                    int age = Convert.ToInt32((DateTime.Now - dateReviewed).TotalDays);
                    //
                    // -- get thumbnail from Spider Docs
                    string thumbnail = "";
                    using (var cs = cp.CSNew()) {
                        if (cs.Open("Spider Docs", $"pageId={page.id}", "id", false, "", 1)) {
                            thumbnail = cs.GetText("thumbnail");
                            if (!string.IsNullOrEmpty(thumbnail)) {
                                thumbnail = $"{cp.Http.CdnFilePathPrefixAbsolute}{thumbnail}";
                            }
                        }
                    }
                    //
                    unsortedPages.Add(new PageReviewPageViewModel {
                        id = page.id,
                        name = cp.Utils.EncodeText(string.IsNullOrEmpty(page.name) ? $"unnamed {page.id}" : page.name),
                        age = age,
                        ageClass = age > 180 ? "age-critical" : age > 90 ? "age-warning" : "age-ok",
                        dateReviewed = dateReviewed.ToString("yyyy-MM-dd"),
                        siteUrl = cp.Content.GetPageLink(page.id),
                        editUrl = cp.Content.GetEditUrl(pageContentId, page.id),
                        thumbnail = thumbnail
                    });
                }
                //
                // -- sort by age descending (oldest reviews first)
                result.pages = unsortedPages.OrderByDescending(p => p.age).ToList();
                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return new PageReviewViewModel { hasPages = false };
            }
        }
    }

    /// <summary>
    /// View model for individual page in the page review
    /// </summary>
    public class PageReviewPageViewModel {
        public int id { get; set; }
        public string name { get; set; }
        public int age { get; set; }
        public string ageClass { get; set; }
        public string dateReviewed { get; set; }
        public string siteUrl { get; set; }
        public string editUrl { get; set; }
        public string thumbnail { get; set; }
        public bool hasThumbnail => !string.IsNullOrEmpty(thumbnail);
    }
}

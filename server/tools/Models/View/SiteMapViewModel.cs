
using Contensive.Addons.Tools.Models.Db;
using Contensive.BaseClasses;
using Contensive.DesignBlockBase.Models.View;
using System;

namespace Models.View {
    public class SiteMapViewModel : DesignBlockViewBaseModel {
        //
        // -- View model properties for Mustache template
        public string title { get; set; }
        public bool showTitle { get; set; }
        public string containerStyleClass { get; set; }
        public string listStyleClass { get; set; }
        public int maxDepth { get; set; }
        public string uniqueId { get; set; }
        public string apiEndpoint { get; set; }
        //
        // ====================================================================================================
        /// <summary>
        /// Populate the view model from the entity model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static SiteMapViewModel create(CPBaseClass cp, SiteMapSettingsModel settings) {
            try {
                //
                // -- base fields
                var result = create<SiteMapViewModel>(cp, settings);
                //
                // -- custom fields
                result.title = settings.title;
                result.showTitle = settings.showTitle;
                result.containerStyleClass = string.IsNullOrWhiteSpace(settings.containerStyleClass)
                    ? "sitemap-container"
                    : settings.containerStyleClass;
                result.listStyleClass = string.IsNullOrWhiteSpace(settings.listStyleClass)
                    ? "sitemap-list"
                    : settings.listStyleClass;
                result.maxDepth = settings.maxDepth;
                //
                // -- generate unique ID for this instance (for JavaScript namespacing)
                result.uniqueId = "sitemap_" + settings.ccguid.Replace("-", "");
                //
                // -- API endpoint for fetching data
                result.apiEndpoint = "/SiteMapData";
                //
                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return null;
            }
        }
    }
}

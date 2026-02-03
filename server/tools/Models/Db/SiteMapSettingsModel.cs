
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;

namespace Contensive.Addons.Tools {
    namespace Models.Db {
        public class SiteMapSettingsModel : Contensive.DesignBlockBase.Models.Db.SettingsBaseModel {
            /// <summary>
            /// table definition
            /// </summary>
            public static readonly DbBaseTableMetadataModel tableMetadata = new DbBaseTableMetadataModel("Site Map Settings", "siteMapsettings", "default", false);
            //====================================================================================================
            //
            // -- Site map widget settings properties
            public string title { get; set; }
            public bool showTitle { get; set; }
            public string containerStyleClass { get; set; }
            public string listStyleClass { get; set; }
            public int maxDepth { get; set; }
            //
            // ====================================================================================================
            /// <summary>
            /// Create or add settings for this design block instance
            /// </summary>
            /// <param name="cp"></param>
            /// <param name="settingsGuid"></param>
            /// <returns></returns>
            public static SiteMapSettingsModel createOrAddSettings(CPBaseClass cp, string settingsGuid) {
                SiteMapSettingsModel result = create<SiteMapSettingsModel>(cp, settingsGuid);
                if ((result == null)) {
                    //
                    // -- create default content
                    result = addDefault<SiteMapSettingsModel>(cp);
                    result.name = tableMetadata.contentName + " " + result.id;
                    result.ccguid = settingsGuid;
                    result.themeStyleId = 0;
                    result.padTop = false;
                    result.padBottom = false;
                    result.padRight = false;
                    result.padLeft = false;
                    //
                    // -- create custom default content
                    result.title = "Site Map";
                    result.showTitle = true;
                    result.containerStyleClass = "sitemap-container";
                    result.listStyleClass = "sitemap-list";
                    result.maxDepth = 0; // 0 = unlimited
                    //
                    result.save(cp);
                    //
                    // -- track the last modified date
                    cp.Content.LatestContentModifiedDate.Track(result.modifiedDate);
                }
                return result;
            }
        }
    }
}

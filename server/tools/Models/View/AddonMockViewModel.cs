
using Contensive.Addons.Tools.Models.Db;
using Contensive.BaseClasses;
using Contensive.DesignBlockBase.Models.View;
using System;

namespace Models.View {
    public class AddonMockViewModel : DesignBlockViewBaseModel {
        // 
        public string jsonData { get; set; }
        public string layout { get; set; }
        // 
        // ====================================================================================================
        /// <summary>
        /// Populate the view model from the entity model
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static AddonMockViewModel create(CPBaseClass cp, AddonMockDataModel settings) {
            try {
                // 
                // -- base fields
                var result = create<AddonMockViewModel>(cp, settings);
                // 
                // -- custom
                result.jsonData = settings.jsondata;
                var layout = Contensive.Models.Db.DbBaseModel.create<Contensive.Models.Db.LayoutModel>(cp, settings.layoutid);
                if (layout==null) {
                    throw new ArgumentException("Layout not found.");
                }
                result.layout = layout.layout.content;
                if (string.IsNullOrEmpty(result.layout)) {
                    throw new ArgumentException("Layout is empty.");
                }
                // 
                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}

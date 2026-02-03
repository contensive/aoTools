
using Contensive.Addons.Tools.Controllers;
using Contensive.Addons.Tools.Models.Db;
using Contensive.BaseClasses;
using Models.View;
using System;

namespace Contensive.Addons.Tools {
    namespace Views {
        //
        // ====================================================================================================
        /// <summary>
        /// Site Map Page Widget - displays a dynamic hierarchical site map
        /// </summary>
        public class SiteMapClass : AddonBaseClass {
            //
            // ====================================================================================================
            //
            public override object Execute(CPBaseClass CP) {
                const string designBlockName = "Site Map";
                try {
                    //
                    // -- read instanceId, guid created uniquely for this instance of the addon on a page
                    var result = string.Empty;
                    var settingsGuid = DesignBlockController.getSettingsGuid(CP, designBlockName, ref result);
                    if ((string.IsNullOrEmpty(settingsGuid)))
                        return result;
                    //
                    // -- locate or create a data record for this guid
                    var settings = SiteMapSettingsModel.createOrAddSettings(CP, settingsGuid);
                    if ((settings == null))
                        throw new ApplicationException("Could not create the design block settings record.");
                    //
                    // -- translate the Db model to a view model and mustache it into the layout
                    var viewModel = SiteMapViewModel.create(CP, settings);
                    if ((viewModel == null))
                        throw new ApplicationException("Could not create design block view model.");
                    result = CP.Mustache.Render(Properties.Resources.SiteMapLayout, viewModel);
                    //
                    // -- if editing enabled, add the link and wrapper
                    return CP.Content.GetEditWrapper(result, SiteMapSettingsModel.tableMetadata.contentName, settings.id);
                } catch (Exception ex) {
                    CP.Site.ErrorReport(ex);
                    return "<!-- " + designBlockName + ", Unexpected Exception -->";
                }
            }
        }
    }
}

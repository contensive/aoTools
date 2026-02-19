
using Contensive.BaseClasses;
using Models.View;
using System;

namespace Contensive.Addons.Tools {
    //
    // ====================================================================================================
    /// <summary>
    /// Site Map Tool - displays a hierarchical site map using layout and Mustache template pattern
    /// </summary>
    public class SiteMapClass : AddonBaseClass {
        //
        // ====================================================================================================
        //
        public override object Execute(CPBaseClass cp) {
            try {
                var form = cp.AdminUI.CreateLayoutBuilder();
                form.title = "Site Map";
                form.description = "Hierarchical view of all active pages in the site.";
                form.isOuterContainer = true;
                //
                // -- create view model and render using layout
                form.body = getMergedLayout(cp);
                //
                // -- assemble form
                return form.getHtml();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        // ====================================================================================================
        /// <summary>
        /// Get the layout and merge with view model using Mustache
        /// </summary>
        private static string getMergedLayout(CPBaseClass cp) {
            try {
                // Load the layout
                string layout = cp.Layout.GetLayout(Constants.layoutSiteMapGuid, Constants.layoutSiteMapName, Constants.layoutSiteMapCdnPathFilename);

                // Create the view model
                var viewModel = SiteMapViewModel.create(cp);

                // Render the layout with the view model
                string result = cp.Mustache.Render(layout, viewModel);

                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return $"<div class=\"alert alert-danger\">Error building site map: {ex.Message}</div>";
            }
        }
    }
}

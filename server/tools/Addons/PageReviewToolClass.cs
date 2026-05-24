
using Contensive.Addons.Tools.Models.View;
using Contensive.BaseClasses;
using System;
using System.Reflection;
//
namespace Contensive.Addons.Tools {
    /// <summary>
    /// Review pages - displays pages as cards sorted by review age
    /// </summary>
    public class PageReviewToolClass : AddonBaseClass {
        //
        // ====================================================================================================
        //
        public override object Execute(CPBaseClass cp) {
            try {
                //
                // -- validate portal environment
                if (!cp.AdminUI.EndpointContainsPortal() && !cp.Request.PathPage.Equals($"/{MethodBase.GetCurrentMethod().DeclaringType.Name}")) {
                    return cp.AdminUI.RedirectToPortalFeature(Constants.guidPortalContentTools, Constants.guidPortalFeaturePageReview, "");
                }
                //
                var form = cp.AdminUI.CreateLayoutBuilder();
                form.title = "Page Review Tool";
                form.description = "Review pages not reviewed in 90 days.";
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
                string layout = cp.Layout.GetLayout(Constants.layoutPageReviewGuid, Constants.layoutPageReviewName, Constants.layoutPageReviewCdnPathFilename);
                var viewModel = PageReviewViewModel.create(cp);
                string result = cp.Mustache.Render(layout, viewModel);
                return result;
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return $"<div class=\"alert alert-danger\">Error building page review: {ex.Message}</div>";
            }
        }
    }
}

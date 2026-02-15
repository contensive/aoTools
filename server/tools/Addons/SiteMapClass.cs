
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contensive.Addons.Tools {
    //
    // ====================================================================================================
    /// <summary>
    /// Site Map Tool - displays a hierarchical site map using AdminUI LayoutBuilder
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
                // -- build the site map tree
                var formBody = new StringBuilder();
                formBody.Append(buildSiteMapTree(cp));
                //
                // -- assemble form
                form.body = formBody.ToString();
                return form.getHtml();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        // ====================================================================================================
        /// <summary>
        /// Build hierarchical site map tree HTML
        /// </summary>
        private string buildSiteMapTree(CPBaseClass cp) {
            try {
                //
                // -- load all active pages
                var allPages = Models.Db.PageContentModel.createList<Models.Db.PageContentModel>(cp, "(active<>0)", "name");
                if (allPages == null || !allPages.Any()) {
                    return "<div class=\"alert alert-info\">No active pages found.</div>";
                }
                //
                // -- organize pages by parent
                var pagesByParent = new Dictionary<int, List<Models.Db.PageContentModel>>();
                foreach (var page in allPages) {
                    int parentId = page.parentId;
                    if (!pagesByParent.ContainsKey(parentId)) {
                        pagesByParent[parentId] = new List<Models.Db.PageContentModel>();
                    }
                    pagesByParent[parentId].Add(page);
                }
                //
                // -- build the tree starting from root pages (parentId = 0)
                var html = new StringBuilder();
                html.Append("<div class=\"sitemap-container\" style=\"padding: 20px;\">");
                html.Append("<ul class=\"sitemap-list\" style=\"list-style-type: none; padding-left: 0;\">");
                buildPageTreeRecursive(cp, pagesByParent, 0, html, 0);
                html.Append("</ul>");
                html.Append("</div>");
                //
                return html.ToString();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                return $"<div class=\"alert alert-danger\">Error building site map: {ex.Message}</div>";
            }
        }
        //
        // ====================================================================================================
        /// <summary>
        /// Recursively build page tree HTML
        /// </summary>
        private void buildPageTreeRecursive(CPBaseClass cp, Dictionary<int, List<Models.Db.PageContentModel>> pagesByParent, int parentId, StringBuilder html, int depth) {
            if (!pagesByParent.ContainsKey(parentId)) {
                return;
            }
            //
            foreach (var page in pagesByParent[parentId]) {
                //
                // -- calculate indentation
                string indent = new string(' ', depth * 4);
                //
                // -- build page item
                html.Append($"{indent}<li style=\"margin: 5px 0; padding-left: {depth * 20}px;\">");
                html.Append($"<span style=\"font-weight: {(depth == 0 ? "bold" : "normal")}; color: #333;\">");
                html.Append($"{cp.Utils.EncodeText(page.name)}");
                html.Append($"</span>");
                html.Append($" <span style=\"color: #999; font-size: 0.9em;\">(ID: {page.id})</span>");
                //
                // -- check if this page has children
                if (pagesByParent.ContainsKey(page.id)) {
                    html.Append($"{indent}<ul style=\"list-style-type: none; padding-left: 0; margin-top: 5px;\">");
                    buildPageTreeRecursive(cp, pagesByParent, page.id, html, depth + 1);
                    html.Append($"{indent}</ul>");
                }
                //
                html.Append($"{indent}</li>");
            }
        }
    }
}

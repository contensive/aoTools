
using Contensive.Addons.Tools.Models.Db;
using Contensive.BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
//
namespace Contensive.Addons.Tools {
    /// <summary>
    /// Review pages
    /// </summary>
    public class PageReviewToolClass : Contensive.BaseClasses.AddonBaseClass {
        //
        private const string buttonRefresh = "Refresh";
        //
        //====================================================================================================
        /// <summary>
        /// addon method, deliver complete Html admin site
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            try {
                var form = cp.AdminUI.CreateLayoutBuilderList();
                form.title = "Page Review Tool";
                form.description = "Review pages not reviewed in 90 days.";
                form.isOuterContainer = true;
                //
                int cnt = cp.Doc.GetInteger("rows per page");
                int age = cp.Doc.GetInteger("Review after this many days");
                string button = cp.Doc.GetText("button");
                //
                StringBuilder formBody = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(button)) {
                    //
                    // -- Process actions
                }
                //
                // Display form
                form.addColumn(new BaseClasses.LayoutBuilder.ReportListColumnBaseClass {
                    caption = "Review Age",
                    captionClass = "",
                    cellClass = "",
                    columnWidthPercent = 15,
                    downloadable = false,
                    name = "Review Age",
                    sortable = false,
                    visible = true
                });
                form.addColumn(new BaseClasses.LayoutBuilder.ReportListColumnBaseClass {
                    caption = "Date Reviewed",
                    captionClass = "",
                    cellClass = "",
                    columnWidthPercent = 25,
                    downloadable = false,
                    name = "Date Modified",
                    sortable = false,
                    visible = true
                });
                form.addColumn(new BaseClasses.LayoutBuilder.ReportListColumnBaseClass {
                    caption = "Edit",
                    captionClass = "",
                    cellClass = "",
                    columnWidthPercent = 10,
                    downloadable = false,
                    name = "Page",
                    sortable = false,
                    visible = true
                });
                form.addColumn(new BaseClasses.LayoutBuilder.ReportListColumnBaseClass {
                    caption = "Public Url",
                    captionClass = "",
                    cellClass = "",
                    columnWidthPercent = 50,
                    downloadable = false,
                    name = "Page",
                    sortable = false,
                    visible = true
                });
                //
                // -- grid
                int pageContentId = cp.Content.GetID(PageContentModel.tableMetadata.contentName);
                List<gridDataModel> unsortedGridData = new();
                foreach (var page in Contensive.Models.Db.DbBaseModel.createList<PageContentModel>(cp, "")) {
                    DateTime dateAdded = (page.dateAdded == null) ? DateTime.Now : (DateTime)page.dateAdded;
                    DateTime dateReviewed = (page.dateReviewed == null) ? dateAdded : (DateTime)page.dateReviewed;
                    unsortedGridData.Add(new gridDataModel {
                        age = Convert.ToInt32((DateTime.Now - dateReviewed).TotalDays),
                        dateReviewed = dateReviewed,
                        editUrl = cp.Content.GetEditUrl(pageContentId, page.id),
                        name = string.IsNullOrEmpty(page.name) ? "unnamed " + page.id : page.name,
                        url = cp.Content.GetPageLink(page.id)
                    });
                }
                unsortedGridData.Sort((x, y) => y.age.CompareTo(x.age));
                foreach (var page in unsortedGridData) {
                    form.addRow();
                    form.setCell(page.age);
                    form.setCell(page.dateReviewed);
                    form.setCell(cp.Html5.A(page.name, new CPBase.BaseModels.HtmlAttributesA { href = page.editUrl, target = "_blank" }));
                    form.setCell(cp.Html5.A(page.url, new CPBase.BaseModels.HtmlAttributesA { href = page.url, target = "_blank" }));
                }
                //
                // -- assemble form
                form.addFormButton(buttonRefresh);
                return form.getHtml();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
    //
    public class gridDataModel {
        public int age { get; set; }
        public DateTime dateReviewed { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string editUrl { get; set; }
    }
}


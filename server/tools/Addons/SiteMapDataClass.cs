
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;
using static Contensive.Addons.Tools.Constants;

namespace Contensive.Addons.Tools {
    namespace Views {
        //
        /// <summary>
        /// Remote method addon that returns site map data as JSON
        /// Endpoint: /SiteMapData
        /// </summary>
        public class SiteMapDataClass : AddonBaseClass {
            //
            /// <summary>
            /// addon execute method
            /// </summary>
            /// <param name="cp"></param>
            /// <returns></returns>
            public override object Execute(CPBaseClass cp) {
                try {
                    //
                    // -- optional application object helper for cache, etc.
                    using Controllers.ApplicationController ae = new(cp);
                    //
                    // -- get all pages from the Page Content table
                    List<SiteMapPageModel> pageList = new List<SiteMapPageModel>();

                    // -- query all active pages using DbBaseModel
                    foreach (var page in Models.Db.PageContentModel.createList<Models.Db.PageContentModel>(cp, "(active<>0)", "name")) {
                        pageList.Add(new SiteMapPageModel() {
                            pageId = page.id,
                            pageName = page.name,
                            parentPageId = page.parentId
                        });
                    }

                    //
                    // -- add page data to response
                    ae.responseNodeList.Add(new Controllers.ResponseNodeClass() {
                        dataFor = "siteMapPages",
                        data = pageList
                    });
                    return ae.getResponse();
                } catch (UnauthorizedAccessException) {
                    return Controllers.ApplicationController.getResponseUnauthorized(cp);
                } catch (Exception ex) {
                    cp.Site.ErrorReport(ex);
                    return Controllers.ApplicationController.getResponseServerError(cp);
                }
            }
            //
            // =====================================================================================
            /// <summary>
            /// Site map page data structure for JSON response
            /// </summary>
            [Serializable()]
            public class SiteMapPageModel {
                public int pageId { get; set; }
                public int parentPageId { get; set; }
                public string pageName { get; set; }
            }
        }
    }
}

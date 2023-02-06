
using Contensive.BaseClasses;
using System;
using System.Text;
//
namespace Contensive.Addons.Tools {
    /// <summary>
    /// Basic Cache Tool
    /// </summary>
    public class CacheToolClass : Contensive.BaseClasses.AddonBaseClass {
        //
        private const string buttonCancel = "Cancel";
        private const string buttonCacheGet = "Get Cache";
        private const string buttonCacheStore = "Store Cache";
        private const string buttonCacheInvalidate = "Invalidate Cache";
        private const string buttonCacheInvalidateAll = "Invalidate All Cache";
        //
        //====================================================================================================
        /// <summary>
        /// addon method, deliver complete Html admin site
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            try {
                var form = new PortalFramework.LayoutBuilderSimple {
                    title = "Cache Tool",
                    description = "Use this tool to get/store/invalidate the application's cache.",
                    isOuterContainer = true
                };
                //
                string cacheKey = cp.Doc.GetText("cacheKey");
                string cacheValue = cp.Doc.GetText("cacheValue");
                string button = cp.Doc.GetText("button");
                //
                StringBuilder formBody = new();
                if (button == buttonCacheGet) {
                    //
                    // -- Get Cache
                    formBody.Append("<div>" + DateTime.Now.ToString() + " cache.getObject(" + cacheKey + ")</div>");
                    object resultObj = cp.Cache.GetObject(cacheKey);
                    if (resultObj == null) {
                        formBody.Append("<div>" + DateTime.Now.ToString() + " NULL returned</div>");
                    } else {
                        try {
                            cacheValue = Newtonsoft.Json.JsonConvert.SerializeObject(resultObj);
                            formBody.Append("<div>" + DateTime.Now.ToString() + "CacheValue object returned, json serialized, length [" + cacheValue.Length + "]</div>");
                        } catch (Exception ex) {
                            formBody.Append("<div>" + DateTime.Now.ToString() + " exception during serialization, ex [" + ex + "]</div>");
                        }
                    }
                    formBody.Append("<p>" + DateTime.Now.ToString() + " Done</p>");
                } else if (button == buttonCacheStore) {
                    //
                    // -- Store Cache
                    formBody.Append("<div>" + DateTime.Now.ToString() + " cache.store(" + cacheKey + "," + cacheValue + ")</div>");
                    cp.Cache.Store(cacheKey, cacheValue);
                    formBody.Append("<p>" + DateTime.Now.ToString() + " Done</p>");
                } else if (button == buttonCacheInvalidate) {
                    //
                    // -- Invalidate
                    cacheValue = "";
                    formBody.Append("<div>" + DateTime.Now.ToString() + " cache.Invalidate(" + cacheKey + ")</div>");
                    cp.Cache.Invalidate(cacheKey);
                    formBody.Append("<p>" + DateTime.Now.ToString() + " Done</p>");
                } else if (button == buttonCacheInvalidateAll) {
                    //
                    // -- Store Cache
                    cacheValue = "";
                    formBody.Append("<div>" + DateTime.Now.ToString() + " cache.InvalidateAll()</div>");
                    cp.Cache.InvalidateAll();
                    formBody.Append("<p>" + DateTime.Now.ToString() + " Done</p>");
                }
                //
                // Display form
                {
                    //
                    // -- cache key
                    formBody.Append(cp.Html5.H4("Cache Key"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetTextEditor("cacheKey", cacheKey, "cacheKey", false)));
                }
                {
                    //
                    // -- cache value
                    formBody.Append(cp.Html5.H4("Cache Value"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetTextEditor("cacheValue", cacheValue, "cacheValue", false)));
                }
                //
                // -- assemble form
                form.body = formBody.ToString();
                form.addFormButton(buttonCancel);
                form.addFormButton(buttonCacheGet);
                form.addFormButton(buttonCacheStore);
                form.addFormButton(buttonCacheInvalidate);
                form.addFormButton(buttonCacheInvalidateAll);
                return form.getHtml(cp);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}


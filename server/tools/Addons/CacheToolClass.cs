
using Contensive.BaseClasses;
using System;
using System.Text;
//
namespace Contensive.Addons.Tools {
    /// <summary>
    /// Cache Tool - Admin utility for managing application cache
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
                var form = cp.AdminUI.CreateLayoutBuilder();
                form.title = "Cache Tool";
                form.description = "Use this tool to get/store/invalidate the application's cache.";
                form.isOuterContainer = true;
                //
                string cacheKey = cp.Doc.GetText("cacheKey");
                string cacheValue = cp.Doc.GetText("cacheValue");
                string button = cp.Doc.GetText("button");
                //
                StringBuilder formBody = new();

                // Process button actions
                if (button == buttonCacheGet) {
                    // -- Get Cache
                    if (string.IsNullOrWhiteSpace(cacheKey)) {
                        formBody.Append(cp.Html5.Div("Error: Cache key is required", "alert alert-danger"));
                    } else {
                        string timestamp = DateTime.Now.ToString();
                        string encodedKey = cp.Utils.EncodeText(cacheKey);
                        formBody.Append(cp.Html5.Div($"{timestamp} cache.getObject({encodedKey})"));

                        object resultObj = cp.Cache.GetObject(cacheKey);
                        if (resultObj == null) {
                            formBody.Append(cp.Html5.Div($"{timestamp} NULL returned"));
                        } else {
                            try {
                                cacheValue = cp.JSON.Serialize(resultObj);
                                formBody.Append(cp.Html5.Div($"{timestamp} CacheValue object returned, json serialized, length [{cacheValue.Length}]"));
                            } catch (Exception ex) {
                                string encodedError = cp.Utils.EncodeText(ex.Message);
                                formBody.Append(cp.Html5.Div($"{timestamp} exception during serialization, ex [{encodedError}]", "alert alert-warning"));
                            }
                        }
                        formBody.Append(cp.Html5.P($"{timestamp} Done"));
                    }
                } else if (button == buttonCacheStore) {
                    // -- Store Cache
                    if (string.IsNullOrWhiteSpace(cacheKey)) {
                        formBody.Append(cp.Html5.Div("Error: Cache key is required", "alert alert-danger"));
                    } else {
                        string timestamp = DateTime.Now.ToString();
                        string encodedKey = cp.Utils.EncodeText(cacheKey);
                        string encodedValue = cp.Utils.EncodeText(cacheValue);
                        formBody.Append(cp.Html5.Div($"{timestamp} cache.store({encodedKey}, {encodedValue})"));

                        cp.Cache.Store(cacheKey, cacheValue);
                        formBody.Append(cp.Html5.P($"{timestamp} Done"));
                    }
                } else if (button == buttonCacheInvalidate) {
                    // -- Invalidate Cache
                    if (string.IsNullOrWhiteSpace(cacheKey)) {
                        formBody.Append(cp.Html5.Div("Error: Cache key is required", "alert alert-danger"));
                    } else {
                        cacheValue = "";
                        string timestamp = DateTime.Now.ToString();
                        string encodedKey = cp.Utils.EncodeText(cacheKey);
                        formBody.Append(cp.Html5.Div($"{timestamp} cache.Invalidate({encodedKey})"));

                        cp.Cache.Invalidate(cacheKey);
                        formBody.Append(cp.Html5.P($"{timestamp} Done"));
                    }
                } else if (button == buttonCacheInvalidateAll) {
                    // -- Invalidate All Cache
                    cacheValue = "";
                    string timestamp = DateTime.Now.ToString();
                    formBody.Append(cp.Html5.Div($"{timestamp} cache.InvalidateAll()"));

                    cp.Cache.InvalidateAll();
                    formBody.Append(cp.Html5.P($"{timestamp} Done"));
                }

                // Display form inputs
                formBody.Append(cp.Html5.H4("Cache Key"));
                formBody.Append(cp.Html5.Div(cp.AdminUI.GetTextEditor("cacheKey", cacheKey, "cacheKey", false)));

                formBody.Append(cp.Html5.H4("Cache Value"));
                formBody.Append(cp.Html5.Div(cp.AdminUI.GetTextEditor("cacheValue", cacheValue, "cacheValue", false)));

                // Assemble form
                form.body = formBody.ToString();
                form.addFormButton(buttonCancel);
                form.addFormButton(buttonCacheGet);
                form.addFormButton(buttonCacheStore);
                form.addFormButton(buttonCacheInvalidate);
                form.addFormButton(buttonCacheInvalidateAll);
                return form.getHtml();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}


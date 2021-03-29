
using Contensive.Addons.Tools.Controllers;
using Contensive.Addons.Tools.Models.Db;
using Contensive.BaseClasses;
using Models.View;
using System;

namespace Contensive.Addons.Tools {
    // 
    // ====================================================================================================
    //
    public class AddonMockClass : AddonBaseClass {
        // 
        // ====================================================================================================
        // 
        public override object Execute(CPBaseClass CP) {
            // 
            // -- read instanceId, guid created uniquely for this instance of the addon on a page
            var result = string.Empty;
            AddonMockDataModel settings = null;
            try {
                //
                // -- this is a dev tool -- catch excerptions and display
                // -- edit tool is outside to let them edit content on exception
                var settingsGuid = DesignBlockController.getSettingsGuid(CP, "Addon Mock", ref result);
                if (string.IsNullOrEmpty(settingsGuid)) {
                    return result;
                }
                // 
                // -- locate or create a data record for this guid
                settings = AddonMockDataModel.createOrAddSettings(CP, settingsGuid);
                if (settings == null) {
                    throw new ApplicationException("Could not create the design block settings record.");
                }
                // 
                // -- translate the Db model to a view model and mustache it into the layout
                var viewModel = AddonMockViewModel.create(CP, settings);
                if (viewModel == null) {
                    throw new ApplicationException("Could not create design block view model.");
                }
                object dataObject = Newtonsoft.Json.JsonConvert.DeserializeObject(viewModel.jsonData);
                result = Nustache.Core.Render.StringToString(viewModel.layout, dataObject);
            } catch (ArgumentException ex) {
                //
                // -- argument error
                result = "<div class=\"ccError\">" + ex.Message + "</div>";
            } catch (ApplicationException ex) {
                //
                // -- app error
                result = "<div class=\"ccError\">" + ex.Message + "</div>";
            } catch (Exception ex) {
                //
                // -- tool, just return the exception
                result = "<div class=\"ccError\">" + ex.Message + "</div>";
            }
            // 
            // -- if editing enabled, add the link and wrapperwrapper
            if (settings == null) { 
                return result; 
            }
            return CP.Content.GetEditWrapper(result, AddonMockDataModel.tableMetadata.contentName, settings.id);
        }
    }
}
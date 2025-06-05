
//using Contensive.BaseClasses;
//using Contensive.Models.Db;
//using System;

//namespace Contensive.Addons.Tools {
//    namespace Models.Db {
//        public class AddonMockDataModel : Contensive.DesignBlockBase.Models.Db.SettingsBaseModel {
//            /// <summary>
//            /// table definition
//            /// </summary>
//            public static DbBaseTableMetadataModel tableMetadata { get; } = new DbBaseTableMetadataModel("Add-on Mock Data", "ccAddonMockData", "default", false);
//            //
//            //====================================================================================================
//            //
//            /// <summary>
//            /// json string pasted in the settings record to simulate server code
//            /// </summary>
//            public string jsondata { get; set; }
//            /// <summary>
//            /// the layout to be merged with the dataq
//            /// </summary>
//            public int layoutid { get; set; }
//            // 
//            // ====================================================================================================
//            public static AddonMockDataModel createOrAddSettings(CPBaseClass cp, string settingsGuid) {
//                AddonMockDataModel result = create<AddonMockDataModel>(cp, settingsGuid);
//                if ((result == null)) {
//                    // 
//                    // -- create default content
//                    result = addDefault<AddonMockDataModel>(cp);
//                    result.name = tableMetadata.contentName + " " + result.id;
//                    result.ccguid = settingsGuid;
//                    result.themeStyleId = 0;
//                    result.padTop = false;
//                    result.padBottom = false;
//                    result.padRight = false;
//                    result.padLeft = false;
//                    // 
//                    // -- create custom content
//                    result.jsondata = "{\"headline\":\"Hello World\",\"items\":[{\"name\":\"Rock\",\"price\":\"$1.24\"},{\"name\":\"Nice Rock\",\"price\":\"$2.99\"}]}";
//                    //
//                    DateTime rightNow = DateTime.Now;
//                    var layout = DbBaseModel.create<LayoutModel>(cp, Constants.guidAddonMockLayout);
//                    string defaultLayout = "<h2>{{headline}}</h2><ul>{{#items}}<hr><li><div>name: {{name}}</div><div>price: {{price}}</div></li>{{/items}}</ul><hr>";
//                    if (layout == null) {
//                        //
//                        // -- no layout found for the default guid, create now
//                        layout = DbBaseModel.addDefault<LayoutModel>(cp);
//                        layout.name = "Addon Mock Default Layout, " + rightNow.Year + rightNow.Month.ToString().PadLeft(2, '0') + rightNow.Day.ToString().PadLeft(2, '0');
//                        layout.ccguid = Constants.guidAddonMockLayout;
//                        layout.layout.content = defaultLayout;
//                        layout.save(cp);
//                    }
//                    if(layout.layout.content != defaultLayout) {
//                        //
//                        // -- layout does not match default content, they edited it so just create a new record
//                        layout = DbBaseModel.addDefault<LayoutModel>(cp);
//                        layout.name = "Addon Mock Default Layout, " + rightNow.Year + rightNow.Month.ToString().PadLeft(2, '0') + rightNow.Day.ToString().PadLeft(2, '0');
//                        layout.ccguid = cp.Utils.CreateGuid();
//                        layout.layout.content = defaultLayout;
//                        layout.save(cp);
//                    }
//                    result.layoutid = layout.id;
//                    // 
//                    result.save(cp);
//                    // 
//                    // -- track the last modified date
//                    cp.Content.LatestContentModifiedDate.Track(result.modifiedDate);
//                }
//                return result;
//            }
//        }
//    }
//}

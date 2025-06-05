
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
//
namespace Contensive.Addons.Tools {
    /// <summary>
    /// Basic Cache Tool
    /// </summary>
    public class SqlSchemaTool : Contensive.BaseClasses.AddonBaseClass {
        //
        public static Dictionary<int, string> tableNameDict = [];
        //
        //====================================================================================================
        /// <summary>
        /// verify the foreign-key relationships in the sql database
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            try {
                var form = cp.AdminUI.CreateLayoutBuilder();
                form.title = "Sql Schema Tool";
                form.description = "This tool documents the Sql Schema tables, fields, keys and descriptions.";
                string bodyCache = cp.PrivateFiles.Read("sqlSchemaToolCache.txt");
                if (!string.IsNullOrEmpty(bodyCache)) {
                    form.addFormButton("Refresh Schema");
                } else {
                    form.addFormButton("Create Schema");
                }
                if (cp.Request.GetText("button") != "Create Schema" && cp.Request.GetText("button") != "Refresh Schema") {
                    //
                    // -- no button pressed, return cached body if it exists
                    form.body = bodyCache;
                } else {
                    //
                    // -- button pressed, create the body
                    StringBuilder body = new();
                    body.Append("<h4>Tables</h4>");
                    body.Append("<ul>");
                    string fieldTypesCsv = "Integer,Short Text,Long Text,Boolean 0/1,Date,File,Lookup,Redirect,Currency,Text File,Image File,Float,Auto Increment Identity,Many To Many,Member Select,CSS File,XML File,JavaScript File,Link,Resource Link,HTML,HTML File,HTML Code,HTML Code File";
                    string[] fieldTypes = fieldTypesCsv.Split(',');
                    string dbFieldTypesCsv = "int,nvarchar(255),nvarchar(max),int,datetime2(7),nvarchar(255),int,int,float,nvarchar(255),nvarchar(255),float,int identity primary key,int,int,nvarchar(255),nvarchar(255),nvarchar(255),nvarchar(255),nvarchar(255),nvarchar(max),nvarchar(255),nvarchar(max),nvarchar(255)";
                    string[] dbFieldTypes = dbFieldTypesCsv.Split(',');
                    foreach (var content in DbBaseModel.createList<ContentModel>(cp, "(active<>0)", "name,id")) {
                        string tableName = getTableName(cp, content.id);
                        if (string.IsNullOrEmpty(tableName)) { continue; }
                        body.Append($"<li><b>{tableName}</b> ({content.name})");
                        body.Append("<ul>");
                        foreach (ContentFieldModel field in DbBaseModel.createList<ContentFieldModel>(cp, "(active>0)and(contentid=" + content.id + ")", "name,id")) {
                            body.Append($"<li>{field.name} ({field.caption})");
                            if (field.name.ToLower() == "id") { body.Append(" (Primary Key)"); }
                            StringBuilder fieldDetails = new();
                            int fieldIndex = field.type - 1;
                            fieldDetails.Append(fieldIndex > -1 && fieldIndex < 25 ? $"<li>{dbFieldTypes[field.type - 1]} (used as {fieldTypes[field.type - 1]})</li>" : "<li>(unknown type)</li>");
                            if (field.type == 7) {
                                string tableNameLookup = getTableName(cp, field.lookupContentId);
                                if (!string.IsNullOrEmpty(tableNameLookup)) { fieldDetails.Append($"<li>Foreign Key referencing {tableNameLookup}.id</li>"); }
                            }
                            if (field.authorable) {
                                fieldDetails.Append($"<li>editable" +
                                    $"{(field.readOnly ? " read-only" : "")}" +
                                    $"{(field.adminOnly ? " admin-only" : "")}" +
                                    $"{(field.developerOnly ? " developer-only" : "")}" +
                                    $"{(string.IsNullOrEmpty(field.defaultValue) ? "" : $" default ({field.defaultValue})")}" +
                                    $"{(field.notEditable ? " write-once" : "")}" +
                                    $"{(field.password ? " password" : "")}" +
                                    $"{(field.required ? " required" : "")}" +
                                    $"{(field.uniqueName ? " unique" : "")}" +
                                    $"</li>");
                            }
                            List<ContentFieldHelpModel> fieldHelpList = DbBaseModel.createList<ContentFieldHelpModel>(cp, $"(fieldId={field.id})");
                            if (fieldHelpList.Count > 0) {
                                string fieldHelp = fieldHelpList.First().helpDefault;
                                fieldDetails.Append(string.IsNullOrWhiteSpace(fieldHelp) ? "" : $"<li>{fieldHelp}</li>");
                            }
                            if (fieldDetails.Length > 0) { body.Append("<ul>" + fieldDetails.ToString() + "</ul>"); }
                            body.Append("</li>");
                        }
                        body.Append("</ul>");
                        body.Append("</li>");
                    }
                    form.body = "</ul>";
                    form.body = body.ToString();
                    cp.PrivateFiles.Save("sqlSchemaToolCache.txt", body.ToString());
                }
                //
                return form.getHtml(cp);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
        //
        //====================================================================================================
        /// <summary>
        /// get the table name for a content record
        /// </summary>
        /// <param name="cp"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        private static string getTableName(CPBaseClass cp, int contentId) {
            if (tableNameDict.ContainsKey(contentId)) { return tableNameDict[contentId]; }
            using (DataTable dt = cp.Db.ExecuteQuery($"select t.name from cccontent c left join cctables t on t.id=c.contentTableId where c.id={contentId}")) {
                if (dt.Rows.Count <= 0) { return ""; }
                string tableName = cp.Utils.EncodeText(dt.Rows[0][0]);
                tableNameDict.Add(contentId, tableName);
                return tableName;
            }
        }
    }
}


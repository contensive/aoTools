
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Text;
//
namespace Contensive.Addons.Tools {
    /// <summary>
    /// Basic Cache Tool
    /// </summary>
    public class CreateSqlForeignKeysTool : Contensive.BaseClasses.AddonBaseClass {
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
                form.title = "Create SQL Foreign Keys";
                form.description = "Use this tool to build the Foreign-Key constraints with NOCHECK in Sql Server that facilitate Schema Diagrams";
                form.addFormButton("Create Foreign Keys");
                //
                // -- disable until constraints are understood
                // -- when enabled, current records are saved during updates, and those records may have constrain
                //
                if (cp.Request.GetText("button") == "Create Foreign Keys") {
                    //
                    form.body = "Adding Foreign Keys to all lookup fields";
                    foreach (var content in DbBaseModel.createList<ContentModel>(cp, "(active<>0)")) {
                        var table = DbBaseModel.create<TableModel>(cp, content.contentTableId);
                        if (table is null) { continue; }
                        foreach (ContentFieldModel field in DbBaseModel.createList<ContentFieldModel>(cp, "(active<>0)and(contentid=" + content.id + ")")) {
                            if (field.type == 7) {
                                //
                                // -- lookup field
                                var foreignContent = DbBaseModel.create<ContentModel>(cp, field.lookupContentId);
                                if (foreignContent is null) { continue; }
                                //
                                var foreignTable = DbBaseModel.create<TableModel>(cp, foreignContent.contentTableId);
                                if (foreignTable is null) { continue; }
                                if (string.IsNullOrEmpty(foreignTable.name)) { continue; }
                                //
                                form.body += $"<br>Add foreignkey constraint between foreignKey {content.name}.{field.name} and {foreignTable}.id";
                                //
                                try {
                                    cp.Db.ExecuteNonQuery($"ALTER TABLE {table.name} WITH NOCHECK ADD CONSTRAINT FK_{table.name}_{field.name}_{foreignTable.name} FOREIGN KEY ({field.name}) REFERENCES {foreignTable.name}(ID)");
                                    break;
                                } catch (Exception ex) {
                                    form.body += $", exception {ex.Message}";
                                }
                                //
                                try {
                                    cp.Db.ExecuteNonQuery($"ALTER TABLE {table.name} NOCHECK CONSTRAINT FK_{table.name}_{field.name}_{foreignTable.name};");
                                    break;
                                } catch (Exception ex) {
                                    form.body += $", exception {ex.Message}";
                                }
                            }
                        }
                    }
                    form.body += "<br>Foreign Keys Created";
                }
                return form.getHtml(cp);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}


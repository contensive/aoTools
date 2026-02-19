
using Contensive.BaseClasses;
using Contensive.Addons.Tools.Controllers;
using System;

namespace Contensive.Addons.Tools {
    /// <summary>
    /// Administrative tool to create SQL Server foreign key constraints for lookup fields
    /// </summary>
    public class CreateSqlForeignKeysTool : Contensive.BaseClasses.AddonBaseClass {
        //
        //====================================================================================================
        /// <summary>
        /// Verify and create foreign-key relationships in the SQL database
        /// </summary>
        /// <param name="cp"></param>
        /// <returns></returns>
        public override object Execute(CPBaseClass cp) {
            try {
                var form = cp.AdminUI.CreateLayoutBuilder();
                form.title = "Create SQL Foreign Keys";
                form.description = "Use this tool to build the Foreign-Key constraints with NOCHECK in SQL Server that facilitate Schema Diagrams";
                form.addFormButton("Create Foreign Keys");
                form.addFormButton("Cancel");

                string button = cp.Doc.GetText("button");

                if (button == "Cancel") {
                    return string.Empty;
                }

                if (button == "Create Foreign Keys") {
                    var result = ForeignKeyController.CreateAllForeignKeys(cp);
                    form.body = result.message;

                    if (!result.success) {
                        form.body = $"<div class='alert alert-warning'>{result.message}</div>";
                    }
                }

                return form.getHtml();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}


using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Text;

namespace Contensive.Addons.Tools.Controllers {
    /// <summary>
    /// Controller for managing SQL Server foreign key constraints
    /// </summary>
    public static class ForeignKeyController {
        private const int FIELD_TYPE_LOOKUP = 7;

        /// <summary>
        /// Result model for foreign key creation operations
        /// </summary>
        public class ForeignKeyResult {
            public bool success { get; set; }
            public string message { get; set; }
            public int keysCreated { get; set; }
            public int keysFailed { get; set; }
        }

        /// <summary>
        /// Creates foreign key constraints for all lookup fields in the database
        /// </summary>
        /// <param name="cp">Contensive platform object</param>
        /// <returns>Result object with success status and summary message</returns>
        public static ForeignKeyResult CreateAllForeignKeys(CPBaseClass cp) {
            var result = new ForeignKeyResult { success = true };
            var output = new StringBuilder();
            int created = 0;
            int failed = 0;

            output.AppendLine("Adding Foreign Keys to all lookup fields<br>");

            foreach (var content in DbBaseModel.createList<ContentModel>(cp, "(active<>0)")) {
                var table = DbBaseModel.create<TableModel>(cp, content.contentTableId);
                if (table == null || string.IsNullOrWhiteSpace(table.name)) continue;

                foreach (var field in DbBaseModel.createList<ContentFieldModel>(cp, $"(active<>0)and(contentid={content.id})")) {
                    if (field.type != FIELD_TYPE_LOOKUP) continue;

                    var foreignContent = DbBaseModel.create<ContentModel>(cp, field.lookupContentId);
                    if (foreignContent == null) continue;

                    var foreignTable = DbBaseModel.create<TableModel>(cp, foreignContent.contentTableId);
                    if (foreignTable == null || string.IsNullOrWhiteSpace(foreignTable.name)) continue;

                    string constraintName = $"FK_{table.name}_{field.name}_{foreignTable.name}";
                    output.Append($"<br>Adding constraint {constraintName} ({content.name}.{field.name} → {foreignTable.name}.id)");

                    try {
                        // Create foreign key with NOCHECK
                        cp.Db.ExecuteNonQuery(@$"
                            IF NOT EXISTS (
                                SELECT 1 FROM sys.foreign_keys 
                                WHERE name = '{constraintName}'
                            )
                            BEGIN
                                ALTER TABLE {table.name}
                                ADD CONSTRAINT {constraintName}
                                FOREIGN KEY ({field.name}) REFERENCES {foreignTable.name}(ID)
                            END
                        ");
                        // Disable constraint checking
                        cp.Db.ExecuteNonQuery($"ALTER TABLE {table.name} NOCHECK CONSTRAINT {constraintName}");

                        output.Append(" ✓");
                        created++;
                    } catch (Exception ex) {
                        output.Append($" ✗ ({ex.Message})");
                        cp.Site.ErrorReport(ex, $"Failed to create foreign key: {constraintName}");
                        failed++;
                    }
                }
            }

            output.AppendLine($"<br><br><strong>Summary:</strong> {created} keys created, {failed} failed");

            result.message = output.ToString();
            result.keysCreated = created;
            result.keysFailed = failed;
            result.success = failed == 0;

            return result;
        }
    }
}

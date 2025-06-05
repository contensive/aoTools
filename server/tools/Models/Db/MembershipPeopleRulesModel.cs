using System.Collections.Generic;
using Contensive.BaseClasses;
using Contensive.Models.Db;
// -- Imports Contensive.Models.Db

namespace Contensive.Addons.Tools.Models.Db {
    public class MembershipPeopleRulesModel : DbBaseModel {
        // 
        // ====================================================================================================
        /// <summary>
        /// table definition
        /// </summary>
        public static DbBaseTableMetadataModel tableMetadata { get; private set; } = new DbBaseTableMetadataModel("Membership People Rules", "mmMembershipPeopleRules", "default", false);
        // 
        // ====================================================================================================
        // -- instance properties
        public int accountId { get; set; }
        public int memberId {
            get; set;
        }
    }
}
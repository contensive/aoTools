using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.Addons.Tools.Addons
{
    public class SubmitInviteProfile : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            var returnObj = new RemoteReturnObj();
            try {
                string email = cp.Doc.GetText("email");
                string username = cp.Doc.GetText("username");
                string firstName = cp.Doc.GetText("firstName");
                string lastName = cp.Doc.GetText("lastName");
                string password = cp.Doc.GetText("password");
                int userId = cp.Doc.GetInteger("userId");

                var user = DbBaseModel.create<PersonModel>(cp, userId);
                user.email = email;
                user.firstName = firstName;
                user.lastName = lastName;
                user.username = username;
                user.password = password;
                user.save(cp);

                returnObj.success = true;
                returnObj.successMessage = "Profile changes saved";
                return returnObj;
            }
            catch(Exception ex) {
                cp.Site.ErrorReport(ex);
                returnObj.success = false;
                returnObj.successMessage = "Profile changes were not able to be saved";
                return returnObj;                
            }
        }

        public class RemoteReturnObj {
            public bool success { get; set; }
            public string successMessage { get; set; }
            public string errorMessage { get; set; }
        }
    }
}

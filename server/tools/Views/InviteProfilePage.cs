using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.Addons.Tools.Views {
    public class InviteProfilePage : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            try {
                var replaceObj = new InviteProfileReplaceObj();
                string encryptedToken = cp.Doc.GetText("token");
                //
                // if there is a token check if it is a valid verification email guid, if it is return the registration form
                var base64EncodedBytes = System.Convert.FromBase64String(encryptedToken);
                string decodedToken = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                string decryptedToken = cp.Security.DecryptTwoWay(decodedToken);

                // get user record 
                var user = PersonModel.create<PersonModel>(cp, Int32.Parse(decryptedToken));
                replaceObj.userEmail = user.email;
                replaceObj.firstName = user.firstName;
                replaceObj.lastName = user.lastName;
                replaceObj.username = user.username;
                replaceObj.userId = user.id.ToString();
                string layout = cp.Layout.GetLayout(Constants.layoutInviteProfileGuid, Constants.layoutInviteProfileName, Constants.layoutInviteProfileCdnPathFilename);
                layout = cp.Mustache.Render(layout, replaceObj);
                cp.Addon.ExecuteAsProcessByUniqueName("Bootstrap");
                return layout;
            }
            catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }

        public class InviteProfileReplaceObj {
            public string userEmail { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string username { get; set; }
            public string userId { get; set; }
        }
    }
}

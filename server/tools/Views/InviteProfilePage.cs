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
                // -- tool is written to expect the user at the keyboard will be the new user account, block an admin who is currently logged in
                if (cp.User.IsRecognized) {
                    cp.User.Logout();
                }
                cp.Addon.ExecuteByUniqueName("Bootstrap");
                //
                // if there is a token check if it is a valid verification email guid, if it is return the registration form
                string encryptedToken = cp.Doc.GetText("token");
                if (string.IsNullOrEmpty(encryptedToken)) { return getMergedLayout(cp, new InviteProfileReplaceObj("The invitation is not valid because the invitation token is blank.")); }
                //
                // -- decode the token
                userInvitationClass userInvitation = null;
                try {
                    var base64EncodedBytes = System.Convert.FromBase64String(encryptedToken);
                    string decodedToken = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                    string decryptedToken = cp.Security.DecryptTwoWay(decodedToken);
                    userInvitation = cp.JSON.Deserialize<userInvitationClass>(decryptedToken);
                } catch (Exception) {
                    return getMergedLayout(cp, new InviteProfileReplaceObj("This invitation is not valid. There was an error reading the required token."));
                }
                if (userInvitation == null) { return getMergedLayout(cp, new InviteProfileReplaceObj("This invitation is not valid. The required token could not be validated.")); }
                if (userInvitation.dateExpires.CompareTo(DateTime.Now)<=0) { return getMergedLayout(cp, new InviteProfileReplaceObj("This invitation is no longer valid.")); }
                //
                // get user record 
                var user = PersonModel.create<PersonModel>(cp, userInvitation.userId);
                if (user is null) { return getMergedLayout(cp, new InviteProfileReplaceObj("This invitation is not valid. The invited user record is not valid.")); }
                //
                var replaceObj = new InviteProfileReplaceObj("") {
                    userEmail = user.email,
                    firstName = user.firstName == "Guest" ? "" : user.firstName,
                    lastName = user.lastName,
                    username = user.username,
                    userId = user.ccguid
                };
                return getMergedLayout(cp, replaceObj);
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }

        private static object getMergedLayout(CPBaseClass cp, InviteProfileReplaceObj replaceObj) {
            string layout = cp.Layout.GetLayout(Constants.layoutInviteProfileGuid, Constants.layoutInviteProfileName, Constants.layoutInviteProfileCdnPathFilename);
            layout = cp.Mustache.Render(layout, replaceObj);
            return layout;
        }

        public class InviteProfileReplaceObj {
            public InviteProfileReplaceObj(string errorMessage) {
                this.errorMessage = errorMessage;
            }
            public string userEmail { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string username { get; set; }
            public string userId { get; set; }
            public string errorMessage { get; set; }
        }
    }
}

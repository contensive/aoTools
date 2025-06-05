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
            try {
                // -- tool is written to expect the user at the keyboard will be the new user account, block an admin who is currently logged in
                if (cp.User.IsRecognized) {
                    cp.User.Logout();
                }
                string userGuid = cp.Doc.GetText("userId");
                if (string.IsNullOrEmpty(userGuid)) { return getErrorResponse("The invited user could not be found. This invitation is no longer valid."); }
                //
                string email = cp.Doc.GetText("email");
                if (string.IsNullOrEmpty(email)) { return getErrorResponse("Email address is required."); }
                //
                var emailDup = DbBaseModel.createFirstOfList<PersonModel>(cp, $"(email={cp.Db.EncodeSQLText(email)})and(ccguid<>{cp.Db.EncodeSQLText(userGuid)})", "id");
                if (emailDup != null) { return getErrorResponse("There is another user with this email address. The changes could not be saved."); }
                //
                string username = cp.Doc.GetText("username");
                if (string.IsNullOrEmpty(username)) { return getErrorResponse("Username is required."); }
                //
                var usernameDup = DbBaseModel.createFirstOfList<PersonModel>(cp, $"(username={cp.Db.EncodeSQLText(username)})and(ccguid<>{cp.Db.EncodeSQLText(userGuid)})", "id");
                if (usernameDup != null) { return getErrorResponse("There is another user with this username. The changes could not be saved."); }
                //
                string password = cp.Doc.GetText("password");
                if (string.IsNullOrEmpty(password)) { return getErrorResponse("Password is required."); }
                //
                var user = DbBaseModel.create<PersonModel>(cp, userGuid);
                if (user is null) { return getErrorResponse("The invited user could not be found. This invitation is no longer valid."); }
                //
                string errorMessage = "";
                if(!cp.User.IsNewLoginOK(user.id, username, password, ref errorMessage) ) { return getErrorResponse(errorMessage); }
                //
                string firstName = cp.Doc.GetText("firstName");
                string lastName = cp.Doc.GetText("lastName");
                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName)) { return getErrorResponse("First and last name are required."); }
                //
                user.email = email;
                user.firstName = firstName;
                user.lastName = lastName;
                user.name = $"{firstName} {lastName}";
                user.username = username;
                user.save(cp);
                //
                if (!cp.User.SetPassword(password, user.id, ref errorMessage)) { return getErrorResponse(errorMessage); }
                //
                return new RemoteReturnObj {
                    success = true,
                    successMessage = "Profile changes saved"
                };
            }
            catch(Exception ex) {
                cp.Site.ErrorReport(ex);
                return getErrorResponse("Profile changes were not able to be saved");
            }
        }
        //
        // ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Returns a JSON object with the error message
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private static object getErrorResponse(string errorMessage) {
            return new RemoteReturnObj {
                success = false,
                errorMessage = errorMessage
            };
        }
        //
        // ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// This class is used to return a JSON object with the success and error messages
        /// </summary>
        public class RemoteReturnObj {
            public bool success { get; set; }
            public string successMessage { get; set; }
            public string errorMessage { get; set; }
        }
    }
}

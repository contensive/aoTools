using Contensive.Addons.Tools.Models.Db;
using Contensive.BaseClasses;
using Contensive.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contensive.Addons.Tools {
    public class InviteUsersTool : Contensive.BaseClasses.AddonBaseClass {
        private const string buttonInviteUsers = "Invite Users";
        private const string buttonResetForm = "Reset Form";

        public override object Execute(CPBaseClass cp) {
            try {
                var form = cp.AdminUI.CreateLayoutBuilder();

                form.title = "Invite Users Tool";
                form.description = "This tool invites users to create a profile on the site. Enter a list of email addresses separated by commas. If no current user exists with this email, a new user will be created. They will be sent an email invitation to add or update their profile.";
                form.isOuterContainer = true;
                form.includeForm = true;
                string inviteUsersList = cp.Doc.GetText("inviteUsersEmailList");
                bool makeUsersAdmin = cp.Doc.GetBoolean("inviteUsersMakeAdmin");
                int addUsersToGroup = cp.Doc.GetInteger("inviteUsersAddToGroup");
                int addUsersToAccount = cp.Doc.GetInteger("inviteUsersAddToAccount");
                int addUsersToOrganization = cp.Doc.GetInteger("inviteUsersAddToOrganization");
                string button = cp.Doc.GetText("button");
                StringBuilder formBody = new();

                if (button.Equals(buttonInviteUsers)) {
                    inviteUsersList = inviteUsersList.Replace("\r\n", ",").Replace("\r", ",").Replace("\n", ",").Replace(",,", ",");
                    List<string> userEmails = inviteUsersList.Split(',').Select(x => x.Trim()).ToList();
                    var userSendList = new StringBuilder();
                    foreach (var email in userEmails) {
                        if (string.IsNullOrEmpty(email)) { continue; }
                        string userSent = email;
                        var newUser = DbBaseModel.createFirstOfList<PersonModel>(cp, $"email={cp.Db.EncodeSQLText(email)}", "id");
                        if (newUser == null) {
                            //add new people record
                            userSent += " (new user)";
                            newUser = DbBaseModel.addDefault<PersonModel>(cp);
                            newUser.email = email;
                        }
                        if (makeUsersAdmin) {
                            userSent += ", set admin";
                            newUser.admin = makeUsersAdmin;
                        }
                        if (addUsersToGroup > 0) {
                            userSent += $", added to group '{cp.Content.GetRecordName("groups", addUsersToGroup)}'";
                            cp.Group.AddUser(addUsersToGroup, newUser.id);
                        }
                        if (addUsersToAccount > 0) {
                            userSent += $", added to account '{cp.Content.GetRecordName("accounts", addUsersToAccount)}'";
                            var ruleList = DbBaseModel.createList<MembershipPeopleRulesModel>(cp, $"(accountid={addUsersToAccount})and(memberId={newUser.id})");
                            if (ruleList.Count == 0) {
                                var rule = DbBaseModel.addDefault<MembershipPeopleRulesModel>(cp);
                                rule.accountId = addUsersToAccount;
                                rule.memberId = newUser.id;
                                rule.save(cp);
                            }
                        }
                        if (addUsersToOrganization > 0) {
                            userSent += $", added to organization '{cp.Content.GetRecordName("organizations", addUsersToOrganization)}'";
                            newUser.organizationId = addUsersToOrganization;
                        }
                        userSendList.Append($"<li>{userSent}</li>");
                        newUser.save(cp);
                        //
                        userInvitationClass userInvitation = new() { userId = newUser.id, dateExpires = DateTime.Now.AddDays(3) };
                        string linkToken = cp.Security.EncryptTwoWay(cp.JSON.Serialize(userInvitation));
                        var encodedLinkStringBytes = System.Text.Encoding.UTF8.GetBytes(linkToken);
                        string encodedLinkString = System.Convert.ToBase64String(encodedLinkStringBytes);
                        string url = cp.Http.WebAddressProtocolDomain + "/InviteProfilePage?token=" + encodedLinkString;
                        //
                        //send the email
                        string emailBody = @$"
                        <p>You have been invited to join {cp.Request.Host}. Click this link to update your profile information: <b><a href=""{url}"">click here</a></b>.";
                        cp.Email.send(email, cp.Email.fromAddressDefault, $"{cp.Request.Host} site invitation", emailBody);
                    }
                    formBody.Append($"<p>{DateTime.Now.ToString()} Invitation sent to:<ul>{userSendList}</ul></p>");
                } else {
                    Dictionary<int, string> groups = [];
                    string getGroupNamesAndIdsSQL = "Select id, name from ccGroups where active>0";
                    using (var cs = cp.CSNew()) {
                        if (cs.OpenSQL(getGroupNamesAndIdsSQL)) {
                            while (cs.OK()) {
                                groups.Add(cs.GetInteger("id"), cs.GetText("name"));
                                cs.GoNext();
                            }
                        }
                    }

                    var accounts = new List<string>();
                    string getAccountsSQL = "Select id, name from abAccounts where active>0";
                    using (var cs = cp.CSNew()) {
                        if (cs.OpenSQL(getAccountsSQL)) {
                            while (cs.OK()) {
                                accounts.Add(cs.GetText("name"));
                                cs.GoNext();
                            }
                        }
                    }

                    var organizations = new List<string>();
                    string getOrganizationsSQL = "Select id, name from organizations where active>0";
                    using (var cs = cp.CSNew()) {
                        if (cs.OpenSQL(getOrganizationsSQL)) {
                            while (cs.OK()) {
                                organizations.Add(cs.GetText("name"));
                                cs.GoNext();
                            }
                        }
                    }


                    formBody.Append(cp.Html5.H4("Email List"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetLongTextEditor("inviteUsersEmailList", "", "inviteUsersEmailList", false), "ms-5"));
                    formBody.Append(cp.Html5.H4("Make user administrator"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetBooleanEditor("inviteUsersMakeAdmin", false, "inviteUsersMakeAdmin", false), "ms-5"));
                    formBody.Append(cp.Html5.H4("Add user to group"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetLookupListEditor("inviteUsersAddToGroup", groups.Values.ToList(), 0, "inviteUsersAddToGroup", false, false), "ms-5"));
                    formBody.Append(cp.Html5.H4("Add user to account"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetLookupListEditor("inviteUsersAddToAccount", accounts, 0, "inviteUsersAddToAccount", false, false), "ms-5"));
                    formBody.Append(cp.Html5.H4("Add user to organization"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetLookupListEditor("inviteUsersAddToOrganization", organizations, 0, "inviteUsersAddToOrganization", false, false), "ms-5"));
                }
                form.body = formBody.ToString();
                form.addFormButton(buttonInviteUsers);
                form.addFormButton(buttonResetForm);
                return form.getHtml();
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
    //
    public class userInvitationClass {
        public int userId { get; set; }
        public DateTime dateExpires { get; set; }

    }
}

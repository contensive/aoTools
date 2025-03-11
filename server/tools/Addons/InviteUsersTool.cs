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

        public override object Execute(CPBaseClass cp) {
            try {
                var form = cp.AdminUI.CreateLayoutBuilder();
                form.title = "Invite Users Tool";
                form.description = "This tool invites users to create a login";

                string inviteUsersList = cp.Doc.GetText("inviteUsersEmailList");
                bool makeUsersAdmin = cp.Doc.GetBoolean("inviteUsersMakeAdmin");
                int addUsersToGroup = cp.Doc.GetInteger("inviteUsersAddToGroup");
                int addUsersToAccount = cp.Doc.GetInteger("inviteUsersAddToAccount");
                int addUsersToOrganization = cp.Doc.GetInteger("inviteUsersAddToOrganization");
                string button = cp.Doc.GetText("button");
                StringBuilder formBody = new();

                if (button.Equals(buttonInviteUsers)) {
                    List<string> userEmails = inviteUsersList.Split(',').Select(x => x.Trim()).ToList();
                    foreach (var email in userEmails) {
                        //add new people record
                        var newUser = PersonModel.addDefault<PersonModel>(cp);
                        newUser.email = email;
                        newUser.admin = makeUsersAdmin;

                        if(addUsersToGroup > 0) {
                            cp.Group.AddUser(addUsersToGroup, newUser.id);
                        }
                        if(addUsersToAccount > 0) {
                            
                        }
                        if(addUsersToOrganization > 0) {
                            newUser.organizationId = addUsersToOrganization;
                        }

                        newUser.save(cp);

                        string linkToken = cp.Security.EncryptTwoWay(newUser.id.ToString());
                        var encodedLinkStringBytes = System.Text.Encoding.UTF8.GetBytes(linkToken);
                        string encodedLinkString = System.Convert.ToBase64String(encodedLinkStringBytes);
                        string url = cp.Http.WebAddressProtocolDomain + "/InviteProfilePage?token=" + encodedLinkString;
                        //
                        //send the email
                        string emailBody = @$"
                        <p>You have been invited to join {cp.Request.Host}. Click this link to update your profile information: <b><a href=""{url}"">click here</a></b>.";
                        cp.Email.send(email, cp.Email.fromAddressDefault, $"{cp.Request.Host} site invitation", emailBody);
                    }
                    formBody.Append("<p>" + DateTime.Now.ToString() + " Invitation sent</p>");
                }
                else {
                    Dictionary<int, string> groups = [];
                    string getGroupNamesAndIdsSQL = "Select id, name from ccGroups";
                    using (var cs = cp.CSNew()) {
                        if (cs.OpenSQL(getGroupNamesAndIdsSQL)) {
                            while (cs.OK()) {
                                groups.Add(cs.GetInteger("id"), cs.GetText("name"));
                                cs.GoNext();
                            }
                        }
                    }

                    var accounts = new List<string>();
                    string getAccountsSQL = "Select id, name from abAccounts";
                    using (var cs = cp.CSNew()) {
                        if (cs.OpenSQL(getAccountsSQL)) {
                            while (cs.OK()) {
                                accounts.Add(cs.GetText("name"));
                                cs.GoNext();
                            }
                        }
                    }

                    var organizations = new List<string>();
                    string getOrganizationsSQL = "Select id, name from organizations";
                    using (var cs = cp.CSNew()) {
                        if (cs.OpenSQL(getOrganizationsSQL)) {
                            while (cs.OK()) {
                                organizations.Add(cs.GetText("name"));
                                cs.GoNext();
                            }
                        }
                    }


                    formBody.Append(cp.Html5.H4("Invite Users Emails"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetLongTextEditor("inviteUsersEmailList", "", "inviteUsersEmailList", false)));
                    formBody.Append(cp.Html5.H4("Make All users Admin"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetBooleanEditor("inviteUsersMakeAdmin", false, "inviteUsersMakeAdmin", false)));
                    formBody.Append(cp.Html5.H4("Add Users to group"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetLookupListEditor("inviteUsersAddToGroup", groups.Values.ToList(), 0, "inviteUsersAddToGroup", false, false)));
                    formBody.Append(cp.Html5.H4("Add users to account"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetLookupListEditor("inviteUsersAddToAccount", accounts, 0, "inviteUsersAddToAccount", false, false)));
                    formBody.Append(cp.Html5.H4("Add users to organizations"));
                    formBody.Append(cp.Html5.Div(cp.AdminUI.GetLookupListEditor("inviteUsersAddToOrganization", organizations, 0, "inviteUsersAddToOrganization", false, false)));
                }
                form.body = formBody.ToString();
                form.addFormButton(buttonInviteUsers);
                return form.getHtml(cp);
            }
            catch (Exception ex) {
                cp.Site.ErrorReport(ex);
                throw;
            }
        }
    }
}
